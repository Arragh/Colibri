# Changelog

## [0.4.3] - 02.03.2026

- Added authorization by claims

## [0.4.1] - 01.03.2026

- (!!!) Updated appsettings.json in README — previous version contained inaccuracies.
- New clusters and routes limits

## [0.4.0] - 28.02.2026

- Added authorization per cluster (early stage implementation). Supports algorithms: RS256, HS256, ES256
- (!) LoadBalancing renamed to LoadBalancer in configuration

## [0.3.1] - 27.02.2026

- Cluster executing bringed to personal middleware
- Added tests for Cluster snapshot builder and routes builder
- Some little fixes

## [0.3.0] - 23.02.2026

- Configuration maked case-insensitive.
- Completed config validation on start/hot-reload.

## [0.2.0] - 23.02.2026

- Allows custom routes via app.Map
- Completed validation clusters.

## [0.1.12] - 21.02.2026

- Completed validation for routes settings.
- Added validation for clusters settings: protocol, hosts.

## [0.1.8] - 19.02.2026

- Validation of route parameters in configuration: correct naming, no duplicates, max count per route (16), max length (1000)
- Validation of clusters in configuration: maximum count (65,535)
- Added CHANGELOG.md
