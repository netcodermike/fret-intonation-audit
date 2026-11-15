# Fret Intonation Audit

Compare measured fretted pitches with equal-tempered target frequencies.

![Sample run from Fret Intonation Audit](docs/demo.webp)

## Run

Install .NET 8 SDK and runtime. Run these commands from the repository root.

```sh
dotnet build --nologo
dotnet bin/Debug/net8.0/App.dll sample.json
```

Replace `sample.json` with a JSON file following the same structure. The app prints a JSON report to standard output.

Each fret sample gives [fret number, measured hertz]. The target is open frequency times 2 raised to fret/12. A sample passes within five cents.

## Verify

Run `python check.py` with Python 3.11 or newer after the build step when applicable. The check verifies the bundled scenario and confirms that an invalid input returns an error.
