"""Fit compact damped-mode/colored-noise percussion models, without retaining PCM."""

import argparse
import json
from pathlib import Path

import numpy as np
from scipy import optimize, signal
from scipy.io import wavfile


CATALOG = {
    "Kick": ("BassDrum", 4),
    "Kick Accent": ("BassDrumAccent", 4),
    "Snare": ("SnareDrum", 3),
    "Snare Accent": ("SnareDrumAccent", 3),
    "Rim Shot": ("RimShot", 4),
    "HiHat": ("HiHat", 0),
    "HiHat Accent": ("HiHatAccent", 0),
    "HiHat Metal": ("MetallicHiHat", 6),
    "Cymbal": ("Cymbal", 0),
    "Bongo High": ("HighBongo", 3),
    "Bongo Low": ("LowBongo", 3),
    "Conga Low": ("LowConga", 3),
    "Cowbell": ("Cowbell", 8),
    "Tamb 1": ("Tambourine", 0),
    "Tamb 2": ("Tambourine2", 0),
    "Guiro 1": ("Guiro", 0),
    "Guiro 2": ("Guiro2", 0),
}


def load(path):
    rate, data = wavfile.read(path)
    if data.dtype.kind == "u":
        data = (data.astype(float) - 128) / 128
    elif data.dtype.kind == "i":
        data = data.astype(float) / (2 ** (data.dtype.itemsize * 8 - 1))
    else:
        data = data.astype(float)
    if data.ndim == 2:
        data = data.mean(axis=1)
    if not np.all(np.isfinite(data)) or not len(data):
        raise ValueError(f"Invalid audio in {path}")
    return rate, data


def modal_wave(mode, t):
    phase = mode["phase"] + 2 * np.pi * (
        mode["frequency"] * t + 0.5 * mode["chirp"] * t * t)
    return mode["amplitude"] * np.exp(-t / mode["decay"]) * np.cos(phase)


def fit_modes(x, rate, count):
    residual = x.copy()
    modes = []
    # Modal fitting runs at a lower rate when the main resonances permit it.
    factor = 4 if count and np.argmax(abs(np.fft.rfft(x))) * rate / len(x) < 2000 else 1
    t = np.arange(len(x)) / rate
    for _ in range(count):
        nfft = max(65536, 2 ** int(np.ceil(np.log2(len(x)))))
        spectrum = abs(np.fft.rfft(residual, nfft))
        frequencies = np.fft.rfftfreq(nfft, 1 / rate)
        peaks, _ = signal.find_peaks(spectrum)
        peaks = [i for i in peaks if 40 < frequencies[i] < min(11000, rate / factor * .43)]
        if not peaks:
            break
        frequency = frequencies[max(peaks, key=lambda i: spectrum[i])]
        y = signal.resample_poly(residual, 1, factor) if factor != 1 else residual
        times = np.arange(len(y)) * factor / rate

        def solve(parameters, output=False):
            f, log_decay, chirp = parameters
            envelope = np.exp(-times / np.exp(log_decay))
            phase = 2 * np.pi * (f * times + chirp * times * times / 2)
            basis = np.column_stack((np.cos(phase) * envelope, np.sin(phase) * envelope))
            weights = np.linalg.lstsq(basis, y, rcond=None)[0]
            return weights if output else basis @ weights - y

        result = optimize.least_squares(
            solve, [frequency, np.log(min(.04, len(x) / rate / 5)), 0],
            bounds=([frequency - min(50, frequency / 5), np.log(.0003), -3000],
                    [frequency + min(50, frequency / 5), np.log(1), 3000]),
            max_nfev=55, ftol=1e-7, xtol=1e-7)
        a, b = solve(result.x, True)
        mode = dict(frequency=float(result.x[0]), decay=float(np.exp(result.x[1])),
                    chirp=float(result.x[2]), amplitude=float(np.hypot(a, b)),
                    phase=float(np.arctan2(-b, a)))
        wave = modal_wave(mode, t)
        if np.sum(wave * wave) < np.sum(x * x) * .002:
            break
        residual -= wave
        modes.append(mode)
    return modes, residual


def envelope(x, rate, scrape):
    # RMS removes the carrier rather than encoding waveform samples.
    width = max(1, int(rate * (.012 if scrape else .002)))
    power = signal.convolve(x * x, np.ones(width) / width, mode="same")
    times = np.unique(np.r_[0, .00025, .0005, .001, .002, .004, .006, .008,
                            .012, .016, .024, .032, .048, .064, .096, .128,
                            np.arange(.16, len(x) / rate, .032), len(x) / rate])
    times = times[times <= len(x) / rate]
    rms = np.sqrt(np.maximum(0, np.interp(times * rate, np.arange(len(x)), power)))
    rms[-1] = 0
    # Sustained scrape amplitude is modulated separately at the measured repetition rate.
    if scrape:
        rms /= np.sqrt(.2734375)  # mean(cos(phase/2)^12)
    return [[float(t), float(a)] for t, a in zip(times, rms)]


def noise_spectrum(residual, rate):
    frequencies, power = signal.periodogram(residual, rate, nfft=65536, detrend=False)
    # Include extra resolution around the narrow analog noise filter resonances.
    knots = np.unique(np.r_[0, np.geomspace(40, min(16000, rate * .48), 32),
                            np.arange(3000, min(12001, rate * .48), 250), rate / 2])
    bandwidth = max(40, rate / 2048)
    gains = []
    for f in knots:
        weights = np.exp(-.5 * ((frequencies - f) / max(bandwidth, f * .025)) ** 2)
        gains.append(float(np.sqrt(np.sum(power * weights) / np.sum(weights))))
    maximum = max(gains)
    return [[float(f), float(g / maximum if maximum else 0)] for f, g in zip(knots, gains)]


def fit_kick_body(x, rate, accent):
    # One fixed carrier: late detuned/chirped residuals must never become a kick tail.
    t = np.arange(len(x)) / rate
    selected = (t >= .008) & (t <= .04)
    if np.count_nonzero(selected) < 2:
        raise ValueError("Kick references must include at least 8 ms of body")
    decay = .037 if accent else .043
    phase = 2 * np.pi * 60 * t[selected]
    envelope = np.exp(-t[selected] / decay)
    basis = np.column_stack((np.cos(phase) * envelope, np.sin(phase) * envelope))
    a, b = np.linalg.lstsq(basis, x[selected], rcond=None)[0]
    return dict(duration=.140 if accent else .160, frequency=60, decay=decay,
                amplitude=float(np.hypot(a, b)), phase=float(np.arctan2(-b, a)))


def synthesize(profile, rate=48000, seed=42):
    length = int(np.ceil(profile["duration"] * rate))
    t = np.arange(length) / rate
    output = sum((modal_wave(m, t) for m in profile["modes"]), np.zeros(length))
    nfft = 2 ** int(np.ceil(np.log2(max(2, length))))
    rng = np.random.default_rng(seed)
    white = rng.normal(size=nfft)
    spectrum = np.asarray(profile["spectrum"])
    gains = np.interp(np.fft.rfftfreq(nfft, 1 / rate), spectrum[:, 0], spectrum[:, 1], right=0)
    noise = np.fft.irfft(np.fft.rfft(white) * gains, nfft)[:length]
    noise /= max(np.sqrt(np.mean(noise * noise)), 1e-12)
    env = np.asarray(profile["envelope"])
    noise *= np.interp(t, env[:, 0], env[:, 1])
    if profile["scrapeRate"]:
        switch = profile["scrapeSwitch"]
        cycles = profile["scrapeRate"] * np.minimum(t, switch)
        cycles += profile["scrapeRate2"] * np.maximum(0, t - switch)
        noise *= np.maximum(0, np.cos(2 * np.pi * (cycles - profile["scrapePhase"]))) ** profile["scrapePower"]
    output += noise
    kick = profile.get("kickBody")
    if kick:
        length = int(np.ceil(kick["duration"] * rate))
        t, output = t[:length], output[:length]
        blend = np.clip((t - .004) / .008, 0, 1)
        blend = blend * blend * (3 - 2 * blend)
        body = kick["amplitude"] * np.exp(-t / kick["decay"]) * np.cos(
            kick["phase"] + 2 * np.pi * kick["frequency"] * t)
        output = output * (1 - blend) + body * blend
    duration = kick["duration"] if kick else profile["duration"]
    output *= np.minimum(1, t / .00002) * np.minimum(1, (duration - t) / (.012 if kick else .002))
    return output


def features(x, rate):
    f, p = signal.periodogram(x, rate, detrend=False)
    energy = np.cumsum(x * x)
    if energy[-1] == 0:
        raise ValueError("Silent sound")
    return dict(centroid=float(np.sum(f * p) / np.sum(p)),
                energy90=float(np.searchsorted(energy, energy[-1] * .9) / rate),
                rms=float(np.sqrt(np.mean(x * x))),
                peak=float(np.max(abs(x))))


def compare(reference, rate, rendered, rendered_rate):
    if rate != rendered_rate:
        rendered = signal.resample_poly(rendered, rate, rendered_rate)
    length = max(len(reference), len(rendered))
    reference = np.pad(reference, (0, length - len(reference)))
    rendered = np.pad(rendered, (0, length - len(rendered)))

    def cosine(a, b):
        return float(np.dot(a, b) / max(np.linalg.norm(a) * np.linalg.norm(b), 1e-20))

    def spectral_bands(x):
        f, p = signal.periodogram(x, rate, nfft=131072, detrend=False)
        weights = np.exp(-.5 * (np.log(np.maximum(f, 1)[None, :] /
                          np.geomspace(40, 16000, 48)[:, None]) / .12) ** 2)
        return np.sqrt(weights @ p)

    def rms_envelope(x):
        width = int(rate * .002)
        x = np.pad(x, (0, (-len(x)) % width))
        return np.sqrt(np.mean(x.reshape(-1, width) ** 2, axis=1))

    return dict(spectralCosine=cosine(spectral_bands(reference), spectral_bands(rendered)),
                envelopeCosine=cosine(rms_envelope(reference), rms_envelope(rendered)))


def fit_scrape(profile, x, rate, long_scrape):
    width = max(1, int(rate * .001))
    rms = np.sqrt(np.mean(x[:len(x) // width * width].reshape(-1, width) ** 2, axis=1))
    times = (np.arange(len(rms)) + .5) * width / rate
    env = np.asarray(profile["envelope"])
    macro = np.interp(times, env[:, 0], env[:, 1])
    rms /= np.linalg.norm(rms)

    def objective(p):
        first, second, switch, phase, power = p
        cycles = first * np.minimum(times, switch) + second * np.maximum(0, times - switch)
        modeled = macro * np.maximum(0, np.cos(2 * np.pi * (cycles - phase))) ** power
        modeled /= max(np.linalg.norm(modeled), 1e-12)
        return modeled - rms

    result = optimize.least_squares(
        objective, [74.8, 125 if long_scrape else 74.8, .545 if long_scrape else .3, .17, 6],
        bounds=([72, 120 if long_scrape else 72, .50 if long_scrape else .29, 0, 1],
                [78, 130 if long_scrape else 78, .59 if long_scrape else .4, .5, 12]),
        max_nfev=150)
    (profile["scrapeRate"], profile["scrapeRate2"], profile["scrapeSwitch"],
     profile["scrapePhase"], profile["scrapePower"]) = map(float, result.x)
    phase = np.linspace(0, 2 * np.pi, 16384, endpoint=False)
    mean_square = np.mean(np.maximum(0, np.cos(phase)) ** (2 * profile["scrapePower"]))
    env[:, 1] *= np.sqrt(.2734375 / mean_square)
    profile["envelope"] = env.tolist()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("sounds", type=Path)
    parser.add_argument("--output", type=Path, required=True, help="Fitted JSON model destination")
    parser.add_argument("--report", type=Path, required=True, help="Measured comparison JSON destination")
    parser.add_argument("--render", type=Path, help="Optional synthesized listening WAV directory")
    parser.add_argument("--compare", type=Path, help="Compare actual .NET stock renders instead of refitting")
    parser.add_argument("--only", action="append", choices=CATALOG, help="Refit one named source, retaining other saved models")
    args = parser.parse_args()
    if args.only and not args.output.exists() and not args.compare:
        parser.error("--only requires an existing complete output model file")
    profiles = json.loads(args.output.read_text(encoding="utf-8")) if args.only and args.output.exists() else {}
    report = {}
    if args.render:
        args.render.mkdir(parents=True, exist_ok=True)
    for name, (instrument, mode_count) in CATALOG.items():
        if args.only and name not in args.only:
            continue
        rate, x = load(args.sounds / (name + ".wav"))
        # Models are peak-normalized, preserving strike shape, not recording-level differences.
        x /= max(np.max(abs(x)), 1e-12)
        if args.compare:
            rendered_rate, rendered = load(args.compare / (instrument + ".wav"))
            reference_features = features(x, rate)
            model_features = features(rendered, rendered_rate)
            report[name] = dict(reference=reference_features, model=model_features)
            report[name].update(compare(x, rate, rendered, rendered_rate))
            print(f"{name:14} centroid={reference_features['centroid']:.0f}"
                  f" -> {model_features['centroid']:.0f} Hz;"
                  f" T90={reference_features['energy90']:.3f} -> {model_features['energy90']:.3f}s;"
                  f" spectral/envelope cosine={report[name]['spectralCosine']:.3f}"
                  f"/{report[name]['envelopeCosine']:.3f}")
            continue
        modes, residual = fit_modes(x, rate, mode_count)
        scrape = name.startswith("Guiro")
        profile = dict(duration=len(x) / rate, modes=modes,
                       spectrum=noise_spectrum(residual, rate),
                       envelope=envelope(residual, rate, scrape),
                       scrapeRate=74.8 if scrape else 0,
                       scrapeRate2=125 if name == "Guiro 2" else 74.8,
                       scrapeSwitch=.545 if name == "Guiro 2" else 10,
                       scrapePhase=.17, scrapePower=6)
        if scrape:
            fit_scrape(profile, x, rate, name == "Guiro 2")
        if name.startswith("Kick"):
            profile["kickBody"] = fit_kick_body(x, rate, name == "Kick Accent")
            profile["duration"] = max(profile["duration"], profile["kickBody"]["duration"])
        profiles[instrument] = profile
        rendered = synthesize(profile, rate)
        reference_features = features(x, rate)
        model_features = features(rendered, rate)
        report[name] = dict(reference=reference_features, model=model_features,
                            modalEnergyFraction=float(1 - np.sum(residual**2) / np.sum(x**2)))
        print(f"{name:14} modes={len(modes)} centroid={reference_features['centroid']:.0f}"
              f" -> {model_features['centroid']:.0f} Hz;"
              f" T90={reference_features['energy90']:.3f} -> {model_features['energy90']:.3f}s",
              flush=True)
        if args.render:
            rendered /= max(1, np.max(abs(rendered)))
            wavfile.write(args.render / (name + ".wav"), rate, (rendered * 32767).astype(np.int16))
    if not args.compare:
        args.output.write_text(json.dumps(profiles, indent=2) + "\n", encoding="utf-8")
    args.report.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")


if __name__ == "__main__":
    main()
