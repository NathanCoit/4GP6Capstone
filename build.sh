#! /bin/sh

# NOTE the command args below make the assumption that your Unity project folder is
#  a subdirectory of the repo root directory, e.g. for this repo "unity-ci-test" 
#  the project folder is "UnityProject". If this is not true then adjust the 
#  -projectPath argument to point to the right location.

## Run the editor unit tests
echo "Running editor unit tests for UnityProject"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
	-batchmode \
	-nographics \
	-silent-crashes \
	-projectPath "$(pwd)/UnityProject" \
	-runTests \
	-testResults $(pwd)/test.xml \
	-quit

## Run the editor unit tests
echo "Running editor unit tests for UnityProject"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
	-batchmode \
	-nographics \
	-silent-crashes \
    -testPlatform playmode \
	-projectPath "$(pwd)/UnityProject" \
	-runTests \
	-testResults $(pwd)/test.xml \
	-quit

rc0=$?
echo "Unit test logs"
cat $(pwd)/test.xml
# exit if tests failed
if [ $rc0 -ne 0 ]; then { echo "Failed unit tests"; exit $rc0; } fi

exit $(($rc1|$rc2))