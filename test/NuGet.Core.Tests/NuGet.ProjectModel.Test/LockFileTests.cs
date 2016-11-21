﻿using System;
using System.Collections.Generic;
using NuGet.Frameworks;
using NuGet.LibraryModel;
using NuGet.Versioning;
using Xunit;

namespace NuGet.ProjectModel.Test
{
    public class LockFileTests
    {
        [Fact]
        public void LockFile_ComparesEqualTools()
        {
            // Arrange
            var lockFileA = new LockFile
            {
                Tools =
                {
                    new LockFileTarget
                    {
                        TargetFramework = FrameworkConstants.CommonFrameworks.NetStandard12,
                        Libraries = { new LockFileTargetLibrary { Name = "SomeLibrary" } }
                    }
                }
            };

            // same thing
            var lockFileB = new LockFile
            {
                Tools =
                {
                    new LockFileTarget
                    {
                        TargetFramework = FrameworkConstants.CommonFrameworks.NetStandard12,
                        Libraries = { new LockFileTargetLibrary { Name = "SomeLibrary" } }
                    }
                }
            };

            // Act & Assert
            Assert.True(lockFileA.Equals(lockFileB), "The two lock files should be equal.");
        }

        [Fact]
        public void LockFile_ComparesDifferentTools()
        {
            // Arrange
            var lockFileA = new LockFile
            {
                Tools =
                {
                    new LockFileTarget
                    {
                        TargetFramework = FrameworkConstants.CommonFrameworks.NetStandard12,
                        Libraries = { new LockFileTargetLibrary { Name = "SomeLibrary" } }
                    }
                }
            };

            // different thing
            var lockFileB = new LockFile
            {
                Tools =
                {
                    new LockFileTarget
                    {
                        TargetFramework = FrameworkConstants.CommonFrameworks.NetStandard12,
                        Libraries = { new LockFileTargetLibrary { Name = "SomeLibrary" } }
                    },
                    new LockFileTarget
                    {
                        TargetFramework = FrameworkConstants.CommonFrameworks.NetStandard12,
                        Libraries = { new LockFileTargetLibrary { Name = "OtherLibrary" } }
                    }
                }
            };

            // Act & Assert
            Assert.False(lockFileA.Equals(lockFileB), "The two lock files should not be equal.");
        }

        [Fact]
        public void LockFile_ComparesEqualProjectFileToolGroups()
        {
            // Arrange
            var lockFileA = new LockFile
            {
                ProjectFileToolGroups =
                {
                    new ProjectFileDependencyGroup("FrameworkA", new [] { "DependencyA" })
                }
            };

            // same thing
            var lockFileB = new LockFile
            {
                ProjectFileToolGroups =
                {
                    new ProjectFileDependencyGroup("FrameworkA", new [] { "DependencyA" })
                }
            };

            // Act & Assert
            Assert.True(lockFileA.Equals(lockFileB), "The two lock files should be equal.");
        }

        [Fact]
        public void LockFile_ComparesDifferentProjectFileToolGroups()
        {
            // Arrange
            var lockFileA = new LockFile
            {
                ProjectFileToolGroups = 
                {
                    new ProjectFileDependencyGroup("FrameworkA", new [] { "DependencyA" })
                }
            };

            // different thing
            var lockFileB = new LockFile
            {
                ProjectFileToolGroups =
                {
                    new ProjectFileDependencyGroup("FrameworkA", new [] { "DependencyA" }),
                    new ProjectFileDependencyGroup("FrameworkB", new [] { "DependencyB" })
                }
            };

            // Act & Assert
            Assert.False(lockFileA.Equals(lockFileB), "The two lock files should not be equal.");
        }

        [Fact]
        public void LockFile_IsValidForPackageSpec_DifferentToolVersions()
        {
            // Arrange
            var lockFile = new LockFile
            {
                ProjectFileDependencyGroups =
                {
                    new ProjectFileDependencyGroup("", new string[0])
                },
                ProjectFileToolGroups =
                {
                    new ProjectFileDependencyGroup(LockFile.ToolFramework.ToString(), new [] { "DependencyA >= 2.0.0" })
                }
            };

            var packageSpec = new PackageSpec()
            {
                Dependencies = new List<LibraryDependency>(),
                Tools = new List<ToolDependency>
                {
                    new ToolDependency
                    {
                        LibraryRange = new LibraryRange
                        {
                            Name = "DependencyA",
                            TypeConstraint = LibraryDependencyTarget.Package,
                            VersionRange = VersionRange.Parse("1.0.0")
                        }
                    }
                }
            };

            // Act & Assert
            Assert.False(lockFile.IsValidForPackageSpec(packageSpec));
        }

        [Fact]
        public void LockFile_IsValidForPackageSpec_MissingTools()
        {
            // Arrange
            var lockFile = new LockFile
            {
                ProjectFileDependencyGroups =
                {
                    new ProjectFileDependencyGroup("", new string[0])
                },
                ProjectFileToolGroups =
                {
                    new ProjectFileDependencyGroup(LockFile.ToolFramework.ToString(), new [] { "DependencyA >= 2.0.0" })
                }
            };

            var packageSpec = new PackageSpec()
            {
                Dependencies = new List<LibraryDependency>(),
                Tools = new List<ToolDependency>
                {
                    new ToolDependency
                    {
                        LibraryRange = new LibraryRange
                        {
                            Name = "DependencyA",
                            TypeConstraint = LibraryDependencyTarget.Package,
                            VersionRange = VersionRange.Parse("1.0.0")
                        }
                    },
                    new ToolDependency
                    {
                        LibraryRange = new LibraryRange
                        {
                            Name = "DependencyB",
                            TypeConstraint = LibraryDependencyTarget.Package,
                            VersionRange = VersionRange.Parse("2.0.0")
                        }
                    }
                }
            };

            // Act & Assert
            Assert.False(lockFile.IsValidForPackageSpec(packageSpec));
        }

        [Fact]
        public void LockFile_IsValidForPackageSpec_ExtraTools()
        {
            // Arrange
            var lockFile = new LockFile
            {
                ProjectFileDependencyGroups =
                {
                    new ProjectFileDependencyGroup("", new string[0])
                },
                ProjectFileToolGroups =
                {
                    new ProjectFileDependencyGroup(LockFile.ToolFramework.ToString(), new []
                    {
                        "DependencyA >= 1.0.0",
                        "DependencyB >= 2.0.0",
                    })
                }
            };

            var packageSpec = new PackageSpec()
            {
                Dependencies = new List<LibraryDependency>(),
                Tools = new List<ToolDependency>
                {
                    new ToolDependency
                    {
                        LibraryRange = new LibraryRange
                        {
                            Name = "DependencyA",
                            TypeConstraint = LibraryDependencyTarget.Package,
                            VersionRange = VersionRange.Parse("1.0.0")
                        }
                    }
                }
            };

            // Act & Assert
            Assert.False(lockFile.IsValidForPackageSpec(packageSpec));
        }

        [Fact]
        public void LockFile_IsValidForPackageSpec_InvalidToolFramework()
        {
            // Arrange
            var lockFile = new LockFile
            {
                ProjectFileDependencyGroups =
                {
                    new ProjectFileDependencyGroup("", new string[0])
                },
                ProjectFileToolGroups =
                {
                    new ProjectFileDependencyGroup("FrameworkA", new []
                    {
                        "DependencyA >= 1.0.0"
                    })
                }
            };

            var packageSpec = new PackageSpec()
            {
                Dependencies = new List<LibraryDependency>(),
                Tools = new List<ToolDependency>
                {
                    new ToolDependency
                    {
                        LibraryRange = new LibraryRange
                        {
                            Name = "DependencyA",
                            TypeConstraint = LibraryDependencyTarget.Package,
                            VersionRange = VersionRange.Parse("1.0.0")
                        }
                    }
                }
            };

            // Act & Assert
            Assert.False(lockFile.IsValidForPackageSpec(packageSpec));
        }

        [Fact]
        public void LockFile_IsValidForPackageSpec_MatchingTools()
        {
            // Arrange
            var lockFile = new LockFile
            {
                ProjectFileDependencyGroups =
                {
                    new ProjectFileDependencyGroup("", new string[0])
                },
                ProjectFileToolGroups =
                {
                    new ProjectFileDependencyGroup(LockFile.ToolFramework.ToString(), new []
                    {
                        "DependencyA >= 1.0.0",
                        "DependencyB >= 2.0.0",
                    })
                }
            };

            var packageSpec = new PackageSpec()
            {
                Dependencies = new List<LibraryDependency>(),
                Tools = new List<ToolDependency>
                {
                    new ToolDependency
                    {
                        LibraryRange = new LibraryRange
                        {
                            Name = "DependencyB",
                            TypeConstraint = LibraryDependencyTarget.Package,
                            VersionRange = VersionRange.Parse("2.0.0")
                        }
                    },
                    new ToolDependency
                    {
                        LibraryRange = new LibraryRange
                        {
                            Name = "DependencyA",
                            TypeConstraint = LibraryDependencyTarget.Package,
                            VersionRange = VersionRange.Parse("1.0.0")
                        }
                    }
                }
            };

            // Act & Assert
            Assert.True(lockFile.IsValidForPackageSpec(packageSpec));
        }
        
        [Fact]
        public void LockFile_ConsidersEquivalentPackageSpec()
        {
            // Arrange
            Func<LockFile> getLockFile = () =>
            {
                var lockFile = new LockFile();
                lockFile.Version = 2;

                lockFile.PackageSpec = new PackageSpec(new[]
                {
                    new TargetFrameworkInformation
                    {
                        FrameworkName = FrameworkConstants.CommonFrameworks.NetCoreApp10,
                        Dependencies = new[]
                        {
                            new LibraryDependency
                            {
                                LibraryRange = new LibraryRange(
                                    "Microsoft.NETCore.App",
                                    new VersionRange(
                                        minVersion: new NuGetVersion("1.0.1"),
                                        originalString: "1.0.1"),
                                    LibraryDependencyTarget.Package)
                            },
                            new LibraryDependency
                            {
                                LibraryRange = new LibraryRange(
                                    "Microsoft.NET.Sdk",
                                    new VersionRange(
                                        minVersion: new NuGetVersion("1.0.0-alpha-20161104-2"),
                                        originalString: "1.0.0-alpha-20161104-2"),
                                    LibraryDependencyTarget.Package),
                                SuppressParent = LibraryIncludeFlags.All
                            }
                        }
                    }
                })
                {
                    Version = new NuGetVersion("1.0.0"),
                    RestoreMetadata = new ProjectRestoreMetadata
                    {
                        ProjectUniqueName = @"X:\ProjectPath\ProjectPath.csproj",
                        ProjectName = "ProjectPath",
                        ProjectPath = @"X:\ProjectPath\ProjectPath.csproj",
                        OutputPath = @"X:\ProjectPath\obj\",
                        OutputType = RestoreOutputType.NETCore,
                        OriginalTargetFrameworks = new[] { "netcoreapp1.0" },
                        TargetFrameworks = new List<ProjectRestoreMetadataFrameworkInfo>
                    {
                        new ProjectRestoreMetadataFrameworkInfo(NuGetFramework.Parse("netcoreapp1.0"))
                    }
                    }
                };

                return lockFile;
            };

            var self = getLockFile();
            var other = getLockFile();

            // Act & Assert
            Assert.NotSame(self, other);
            Assert.Equal(self, other);
        }
    }
}
