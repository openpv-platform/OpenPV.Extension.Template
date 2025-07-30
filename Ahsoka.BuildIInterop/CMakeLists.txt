# CMakeList.txt : Top-level CMake project file, do global configuration
# and include sub-projects here.
#
cmake_minimum_required (VERSION 3.8)

project ("Ahsoka.Interop")

add_subdirectory ("Ahsoka.Proto")

# MOVE Outputs to Normal Location so not in Ahsoka Output.
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_OUTPUTS})
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_OUTPUTS})
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_OUTPUTS})

# Add Cairo Interop build
IF (${AHSOKA_BUILD_OUTPUTS} MATCHES "OpenViewLinux_TargetSupport/SDK$")
    add_subdirectory ("Ahsoka.VideoPlayer")
ENDIF()
IF (${AHSOKA_BUILD_OUTPUTS} MATCHES "OpenViewLinuxPro_TargetSupport/SDK$")
    add_subdirectory ("Ahsoka.VideoPlayer") 
ENDIF()



