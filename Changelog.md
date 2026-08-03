# 2.0.0

**Supported Revit versions are now 2025, 2026 and 2027.** Support for Revit 2020-2024 has been dropped; stay on 1.1.1 if you need those, though see the fix below before doing so.

- Fixed the add-in failing to load entirely. The manifest referenced an assembly name the build never produced, so every 1.1.1 install (both MSIs and the bundle) registered an add-in Revit could not load. If you installed 1.1.1, uninstall it before installing 2.0.0.
- Added Revit 2026 and 2027 support, with Revit 2027 running on .NET 10.
- The build now verifies that published manifests match the assemblies beside them, so this class of packaging break cannot ship again.

# 1.1.1

Release with installer on Github
