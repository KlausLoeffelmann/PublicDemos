namespace LayoutTests.App.Services;

public sealed class UselessFacts
{
    public sealed record Fact(int Number, string Type, string Text);

    private static readonly IReadOnlyList<Fact> s_facts = BuildFacts();

    public IReadOnlyList<Fact> All => s_facts;

    public IReadOnlyList<Fact> PickRandom(int count = 20, int? seed = null)
    {
        if (count <= 0)
        {
            return Array.Empty<Fact>();
        }

        int take = Math.Min(count, s_facts.Count);
        Random random = seed.HasValue ? new Random(seed.Value) : Random.Shared;
        return s_facts.OrderBy(_ => random.Next()).Take(take).ToList();
    }

    private static IReadOnlyList<Fact> BuildFacts()
    {
        string[] types =
        {
            "Animal", "History", "Space", "Food", "Science",
            "Geography", "Body", "Tech", "Music", "Sport",
        };

        string[] texts =
        {
            "Octopuses have three hearts and blue blood.",
            "Bananas are berries, but strawberries are not.",
            "Honey never spoils when sealed in a jar.",
            "A group of flamingos is called a 'flamboyance'.",
            "The Eiffel Tower can be 15 cm taller in summer.",
            "Wombat poop is cube-shaped.",
            "Sharks existed before trees.",
            "Cows have best friends and get stressed when separated.",
            "Venus is the only planet that rotates clockwise.",
            "A bolt of lightning is hotter than the surface of the Sun.",
            "Sloths can hold their breath longer than dolphins can.",
            "There are more stars in the universe than grains of sand on Earth.",
            "A day on Venus is longer than a year on Venus.",
            "Cats have over 20 muscles that control their ears.",
            "Slugs have four noses.",
            "The shortest war in history lasted 38 minutes.",
            "Octopus arms have their own neurons.",
            "Pineapple takes about two years to grow.",
            "Hot water freezes faster than cold under specific conditions.",
            "A jiffy is an actual unit of time: 1/100th of a second.",
            "The Hawaiian alphabet has only 13 letters.",
            "An ostrich's eye is bigger than its brain.",
            "Cleopatra lived closer in time to the Moon landing than to the building of the pyramids.",
            "Tomato ketchup was sold in the 1830s as medicine.",
            "Wombats can run up to 40 km/h in short bursts.",
            "Bats are the only mammals capable of true flight.",
            "Humans share 60% of their DNA with bananas.",
            "Nintendo was founded in 1889 as a playing-card company.",
            "There are 86,400 seconds in a day.",
            "The longest English word without a vowel is 'rhythms'.",
            "Polar bear fur is not white; it is transparent.",
            "Mosquitoes are attracted to people who just ate bananas.",
            "Spiders have transparent blood.",
            "The fingerprints of koalas are nearly indistinguishable from humans'.",
            "Lemons float, but limes sink.",
            "Goats have rectangular pupils.",
            "Dolphins call each other by name.",
            "Crows can recognize human faces.",
            "Some turtles can breathe through their butts.",
            "The Mona Lisa has no eyebrows.",
            "Pigs cannot look up into the sky.",
            "Butterflies taste with their feet.",
            "A snail can sleep for three years.",
            "Russia has a larger surface area than Pluto.",
            "An average cloud weighs about 500 tons.",
            "Cashews grow out of a fruit called the cashew apple.",
            "Sound travels four times faster in water than in air.",
            "Sea otters hold hands while sleeping so they do not drift apart.",
            "A 'set' has the most definitions of any word in English.",
            "The unicorn is Scotland's national animal.",
            "The Sun makes up 99.86% of the Solar System's mass.",
            "Bubble wrap was originally invented as wallpaper.",
            "A blue whale's heart is roughly the size of a small car.",
            "The first oranges were not orange; they were green.",
            "There is enough DNA in your body to stretch to Pluto and back 17 times.",
            "Penguins propose with pebbles.",
            "Olympic gold medals are mostly silver.",
            "Camels have three eyelids.",
            "An 'eep' is the sound a baby chicken makes (sort of).",
            "The shortest commercial flight is 1.5 minutes long.",
            "A group of crows is called a 'murder'.",
            "Reindeer eyeballs turn blue in winter.",
            "Most lipstick contains fish scales.",
            "There is a town in Norway called Hell, and it freezes over.",
            "Mantis shrimp can punch with the speed of a .22 caliber bullet.",
            "A snail's mouth is no larger than the head of a pin.",
            "Dragonflies have six legs but cannot walk.",
            "Sea cucumbers eat with their feet.",
            "Avocados are toxic to most birds.",
            "Pluto is smaller than Russia.",
            "Tigers have striped skin, not just striped fur.",
            "Sloths can take up to a month to digest a single meal.",
            "A snail can have over 25,000 teeth.",
            "Frogs cannot vomit; if they need to, they expel the whole stomach.",
            "The longest hiccuping spree lasted 68 years.",
            "Macadamia nuts are toxic to dogs.",
            "Penguins have knees inside their bodies.",
            "Cats sweat through their paw pads.",
            "An ant can lift up to 50 times its own body weight.",
            "There are more fake flamingos in the world than real ones.",
            "The first computer 'bug' was an actual moth.",
            "A flock of crows is sometimes also called a 'storytelling'.",
            "Watermelons are 92% water.",
            "Banging your head against a wall burns about 150 calories an hour.",
            "Saturn would float in water if you could find a bathtub big enough.",
            "Glass takes one million years to decompose naturally.",
            "Pigs orgasms can last up to 30 minutes.",
            "Earthworms have five hearts.",
            "Hippos sweat a red oily substance that protects their skin from the sun.",
            "Lobsters' urine comes out of their faces.",
            "The 'pop' of popcorn happens at exactly 180°C.",
            "Goldfish can see more colors than humans.",
            "Some bamboo plants grow up to 91 cm per day.",
            "There are 293 ways to make change for a US dollar.",
            "A shrimp's heart is in its head.",
            "Hot chocolate tastes better in an orange cup.",
            "Sharks are older than the rings of Saturn.",
            "A group of porcupines is called a 'prickle'.",
            "Most people walk about 120,000 km in a lifetime.",
            "Toasters are responsible for more deaths per year than sharks.",
            "Honey is the only food that includes all the substances necessary to sustain life.",
            "The first webcam watched a coffee pot at Cambridge University.",
        };

        int count = Math.Min(texts.Length, 100);
        List<Fact> facts = new(count);
        for (int i = 0; i < count; i++)
        {
            facts.Add(new Fact(i + 1, types[i % types.Length], texts[i]));
        }

        return facts;
    }
}
