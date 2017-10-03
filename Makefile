
all: 

.PHONY: release
release:
	dotnet publish -f netcoreapp2.0 -c Release LicenseCheck

.PHONY: check
check:
	cd LicenseCheck.Test.Unit && dotnet test
	cd LicenseCheck.Test.Integration && dotnet test

.PHONY: clean
clean:
	git clean -xdf
