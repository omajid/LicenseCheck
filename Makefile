
all: 


.PHONY: check
check:
	cd LicenseCheck.Test.Unit && dotnet test
	cd LicenseCheck.Test.Integration && dotnet test
