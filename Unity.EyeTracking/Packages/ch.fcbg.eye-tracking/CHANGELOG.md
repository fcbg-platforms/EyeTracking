# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [3.0.1] - 2023-08-16
### Fixed
- Small error in vive originWorld position (now use the orientation of the head).


## [3.0.0] - 2023-08-15
### Added
- Package is now released with OpenUPM


## [2.3.1] - 2023-03-09
### Fixed
- Add default constructor for EyeDataOutput


## [2.3.0] - 2022-11-18
### Added
- Now Head data is also a part of the interface.

### Fixed
- Retry starting SRanipal after some times in case that the device is not found (cause it could be found afterwards).


## [2.2.1] - 2022-11-14
### Fixed
- Fix on eye frown and wide output values


## [2.2.0] - 2022-11-11
### Fixed
- Bug that prevented the launch of eyetracking / vr.

### Added
- Object looked label in interface.

### Changed
- For now only use eye tracking inside fixedUpdate


## [2.1.1] - 2022-11-07
### Fixed
- Fix bug that prevented ViveGazeManager registration


## [2.1.0] - 2022-11-01
### Changed
- Remove unused resources
- Remove dependency to SO package

### Fixed
- Unregistrable actions
- constructor for data output


## [2.0.0] - 2022-10-27
### Changed
- Changed the interface API to be more generic and to contain more usefull data.
- Improved performance of Vive eye tracking, and increased the refresh rate (use of FixedUpdate).


## [1.1.1] - 2022-09-20
### Changed
- Eye values are now sampled inside the FixedUpdate callback


## [1.1.0] - 2022-08-25
### Added
- Add GazeHit to the public data


## [1.0.0] - 2022-06-23
First release of the package. Changes made include:
- Support of ViveSR ranipal eye tracking system.
- Support of a Fake Eye tracking system, with mouse control.
