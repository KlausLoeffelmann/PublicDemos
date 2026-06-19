# WinForms Agent Development Guidelines

## Critical: Two Code Contexts

| Context | Files/Location | Language Level | Key Rule |
|---------|----------------|----------------|----------|
| **Designer Code** | `.designer.cs`, inside `InitializeComponent` | Serialization-centric (assume C# 2.0 language features) | Simple, predictable, parsable |
| **Regular Code** | `.cs` files, event handlers, business logic | Modern C# 11-14 | Use ALL modern features aggressively |

**Decision:** In `.designer.cs` or `InitializeComponent` → Designer rules. Otherwise → Modern C# rules.

---

## 🚨 Designer File Rules (TOP PRIORITY)

⚠️ Make sure, Diagnostic Errors and build/compile errors eventually are completely addressed!

### Prohibited in InitializeComponent

| Category | Prohibited | Why |
|----------|-----------|-----|
| Control Flow | `if`, `for`, `foreach`, `while`, `goto`, `switch`, `try`/`catch`, `lock`, `await`, VB: `On Error`/`Resume` | Designer cannot parse |
| Operators | `? :` (ternary), `??`/`?.`/`?[]` (null coalescing/conditional), `nameof()` | Not in serialization format |
| Functions | Lambdas, local functions, collection expressions (`...=[]` or `...=[1,2,3]`) | Breaks Designer parser |

**Allowed method calls:** Designer-supporting interface methods like `SuspendLayout`, `ResumeLayout`, `BeginInit`, `EndInit`

### ❌ Prohibited in .designer.cs File

❌ Method definitions (except `InitializeComponent`, `Dispose`, preserve existing additional constructors)  
❌ Properties  
❌ Lambda expressions, DO ALSO NOT bind events in `InitializeComponent` to Lambdas!
❌ Complex logic
❌ `??`/`?.`/`?[]` (null coalescing/conditional), `nameof()`
❌ Collection Expressions

### ✅ Correct Pattern

✅ File-scope namespace definitions (preferred)

### Required Structure of InitializeComponent Method

| Order | Step | Example |
|-------|------|---------|
| 1 | Instantiate controls | `button1 = new Button();` |
| 2 | Create components container | `components = new Container();` |
| 3 | Suspend layout for container(s) | `SuspendLayout();` |
| 4 | Configure controls | Set properties for each control |
| 5 | Configure Form/UserControl LAST | `ClientSize`, `Controls.Add()`, `Name` |
| 6 | Resume layout(s) | `ResumeLayout(false);` |
| 7 | Backing fields at EOF | After last `#endregion` | `_btnOK`, `_txtFirstname`|

(Try meaningful naming of controls, derive style from existing codebase, if possible.)

```csharp
private void InitializeComponent()
{
    // 1. Instantiate
    _btnOK = new Button();
    _btnCancel = new Button();
    
    // 2. Components
    components = new Container();
    
    // 3. Suspend
    SuspendLayout();
    
    // 4. Configure controls
    _btnOK.Location = new Point(93, 263);
    _btnOK.Name = "_btnOK";
    _btnOK.Size = new Size(114, 68);
    _btnOK.Text = "OK";

    // OK, if BtnOK_Click is defined in main .cs file
    _btnOK.Click += BtnOK_Click;
    
    _btnCancel.Location = new Point(229, 263);
    _btnCancel.Name = "_btnCancel";
    _btnCancel.Size = new Size(114, 68);
    _btnCancel.Text = "Cancel";

    // NOT AT ALL OK, we cannot have Lambdas in InitializeComponent!
    _btnCancel.Click += (s, e) => Close();
    
    // 5. Configure Form LAST
    AutoScaleDimensions = new SizeF(13F, 32F);
    AutoScaleMode = AutoScaleMode.Font;
    ClientSize = new Size(702, 672);
    Controls.Add(_btnOK);
    Controls.Add(_btnCancel);
    Name = "MainForm";
    
    // 6. Resume
    ResumeLayout(false);
}

#endregion

// 7. Backing fields at EOF

private Button _btnOK;
private Button _btnCancel;
```

**Remember:** Complex initialization logic goes in main `.cs` file, NOT `.designer.cs`.
