# SJBR & Associates — Website (v1)

.NET Core MVC site for the CA firm, using JSON files instead of a database.
v1 scope: **Home** page + **Contact** page (with an enquiry form).

## Project structure

```
SJBRAssociatesWeb/
├── Controllers/
│   ├── HomeController.cs      -> loads team/services/contact for Home
│   └── ContactController.cs   -> shows form, saves + emails enquiries
├── Models/                    -> POCOs + the enquiry form's validation rules
├── Services/
│   ├── JsonDataService.cs     -> reads/writes the JSON files ("the database")
│   └── EmailService.cs        -> sends an SMTP email when a form is submitted
├── Views/
│   ├── Home/Index.cshtml
│   ├── Contact/Index.cshtml
│   └── Shared/_Layout.cshtml
├── Data/
│   ├── team.json               -> edit to add/remove team members
│   ├── services.json           -> PLACEHOLDER — replace once you have the real service list
│   ├── contact.json            -> firm details shown on the site
│   └── enquiries.json          -> starts empty; fills up as people submit the form
└── wwwroot/css/site.css
```

## Note on the `Data/` JSON files

The `.csproj` doesn't list `Data/*.json` explicitly — the .NET SDK already
includes every file under the project folder as a default `Content` item and
copies it on publish. Adding them again manually causes a build error
(`NETSDK1022: Duplicate 'Content' items were included`), so leave that alone
unless you deliberately exclude default items elsewhere.

## Requirements

- **.NET 10 SDK** (LTS) — install from https://dotnet.microsoft.com/download/dotnet/10.0 if `dotnet --version` doesn't already show a `10.x` SDK.

## Running it locally

```bash
dotnet restore
dotnet run
```

Then open the URL shown in the console (usually `https://localhost:5001` or similar).

## Before you go live — things to update

1. **`Data/services.json`** — this is placeholder content. The forwarded message said
   "Services: will give separately," so swap this out once you have the real list.
2. **`Data/contact.json`** — email and WhatsApp number were marked "will confirm" in the
   source message. Double-check `info@sjbr.in` and the WhatsApp number before publishing.
3. **`appsettings.json` → `Smtp` section** — do **not** commit a real password here.
   For local dev, use `dotnet user-secrets`:
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "Smtp:Password" "your-real-app-password"
   ```
   On Azure App Service, set `Smtp__Password` (double underscore) as an **Application
   Setting** instead — never in a checked-in file.
   If you use Gmail's SMTP, you'll need an **App Password**, not your normal Gmail
   password (Google blocks plain password SMTP logins).

## ⚠️ Two things that will bite you with JSON-file storage

1. **Concurrent writes.** `JsonDataService.SaveEnquiryAsync` uses a `SemaphoreSlim`
   lock plus a write-to-temp-then-rename swap, so two simultaneous form submissions
   won't corrupt `enquiries.json` or silently drop one of them. If you add more
   JSON-backed writes later, follow the same pattern — don't do a naive
   read-modify-write.
2. **Azure App Service file persistence.** On a single-instance App Service plan,
   writes to local files generally survive app restarts (they live on persistent
   storage), but this gets unreliable the moment you:
   - scale out to multiple instances (each instance gets its own copy — an enquiry
     saved on instance A won't show up if a later request lands on instance B), or
   - use certain deployment slots/swap scenarios.

   For a low-traffic single-instance deployment this setup is fine to start with.
   If you outgrow it, the next step isn't necessarily a full SQL database — Azure
   **Table Storage** or **Blob Storage** are cheap, simple upgrades that keep the
   "just JSON-ish documents" feel while being safe for multi-instance and scale-out
   scenarios.

## Extending it

- Add pages the same way: new `[Page]Controller`, matching `Views/[Page]/Index.cshtml`,
  and a nav link in `Views/Shared/_Layout.cshtml`.
- To view submitted enquiries, either open `Data/enquiries.json` directly, or build a
  simple `/admin/enquiries` page later — happy to help with that when you're ready.
