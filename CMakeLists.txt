cmake_minimum_required(VERSION 3.19)

project(MBUltra)

set(CMAKE_CXX_STANDARD 14)

execute_process(
        COMMAND git rev-parse --abbrev-ref HEAD
        WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
        OUTPUT_VARIABLE GIT_BRANCH
        OUTPUT_STRIP_TRAILING_WHITESPACE
    )

    # Provide the branch name to C++ via a preprocessor definition
    add_definitions("-DGIT_BRANCH=\"${GIT_BRANCH}\"")

include(tools/cmake/CheckGit.cmake)
CheckGitSetup()

add_subdirectory(tools/cmake)
target_link_libraries(MBUltra git_version)

# Defaults to OFF for local developers
option(ENABLE_NIGHTLY_FEATURES "Isolate networking for testing versions" OFF)
set(BUILD_INCREMENT "0" CACHE STRING "Automatic index passed down from GitHub Actions")

if(ENABLE_NIGHTLY_FEATURES)
    target_compile_definitions(MBUltra PRIVATE 
        NIGHTLY_BUILD
        TORQUE_BUILD_OFFSET=${BUILD_INCREMENT}
    )
else()
    target_compile_definitions(MBUltra PRIVATE 
        TORQUE_BUILD_OFFSET=0
    )
endif()