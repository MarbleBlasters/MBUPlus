cmake_minimum_required(VERSION 3.19)

project(MBUltra)

set(CMAKE_CXX_STANDARD 14)

include(tools/cmake/CheckGit.cmake)
CheckGitSetup()

add_subdirectory(tools/cmake)
target_link_libraries(MBUltra git_version)