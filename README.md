# MiniServ

A tiny, unambitious file server - for all the cases where `file://` is not enough
but a full-blown host would be *too* much.

## Features

- Host files from any path on disk
- Automatically resolve `/foo` to `/foo/index.html`
- Redirect a 404 to `/404.html` (opt-in)
- Send `Cache-Control: no-cache; no-store` (opt-in)

## Usage

`miniserv` is available as a [dotnet tool](https://www.nuget.org/packages/MiniServ/):
```sh
dotnet tool install --global miniserv
```

Serve the current directory with default settings:
```sh
miniserv
```

Serve a specific directory with default settings:
```sh
miniserv path/to/content # Can be absolute or relative to the current directory
```
