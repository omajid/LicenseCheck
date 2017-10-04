
all: 

.PHONY: release
release:
	dotnet publish -f netcoreapp2.0 -c Release LicenseCheck

.PHONY: check
check:
	dotnet test LicenseCheck.Test.Unit
	dotnet test LicenseCheck.Test.Integration

.PHONY: clean
clean:
	rm -rf LicenseCheck/bin LicenseCheck/obj
	rm -rf LicenseCheck.Test.Unit/bin LicenseCheck.Test.Unit/obj
	rm -rf LicenseCheck.Test.Integration/bin LicenseCheck.Test.Integration/obj
