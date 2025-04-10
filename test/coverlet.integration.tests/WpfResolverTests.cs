﻿// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Coverlet.Core.Abstractions;
using Coverlet.Core.Instrumentation;
using Coverlet.Tests.Utils;
using Microsoft.Extensions.DependencyModel;
using Moq;
using Xunit;

namespace Coverlet.Integration.Tests
{
  public class WpfResolverTests : BaseTest
  {
    [Fact]
    public void TestInstrument_NetCoreSharedFrameworkResolver()
    {
      Assert.SkipUnless(RuntimeInformation.IsOSPlatform(OSPlatform.Windows), "Test requires Windows");
      string buildConfiguration = TestUtils.GetAssemblyBuildConfiguration().ToString().ToLowerInvariant();
      string wpfProjectPath = TestUtils.GetTestProjectPath("coverlet.tests.projectsample.wpf8");
      string testBinaryPath = Path.Combine(TestUtils.GetTestBinaryPath("coverlet.tests.projectsample.wpf8"), buildConfiguration);
      Assert.Equal(0, DotnetCli($"build \"{wpfProjectPath}\"", out string output, out string error));
      string assemblyLocation = Directory.GetFiles(testBinaryPath, "coverlet.tests.projectsample.wpf8.dll", SearchOption.AllDirectories).First();

      var mockLogger = new Mock<ILogger>();
      var resolver = new NetCoreSharedFrameworkResolver(assemblyLocation, mockLogger.Object);
      var compilationLibrary = new CompilationLibrary(
          "package",
          "System.Drawing",
          "0.0.0.0",
          "sha512-not-relevant",
          Enumerable.Empty<string>(),
          Enumerable.Empty<Dependency>(),
          true);

      var assemblies = new List<string>();
      Assert.True(resolver.TryResolveAssemblyPaths(compilationLibrary, assemblies),
          "sample assembly shall be resolved");
      Assert.NotEmpty(assemblies);
    }

    [Fact]
    public void TestInstrument_NetCoreSharedFrameworkResolver_SelfContained()
    {
      Assert.SkipUnless(RuntimeInformation.IsOSPlatform(OSPlatform.Windows), "Test requires Windows");
      string buildConfiguration = TestUtils.GetAssemblyBuildConfiguration().ToString().ToLowerInvariant();
      string wpfProjectPath = TestUtils.GetTestProjectPath("coverlet.tests.projectsample.wpf8.selfcontained");
      string testBinaryPath = Path.Combine(TestUtils.GetTestBinaryPath("coverlet.tests.projectsample.wpf8.selfcontained"), $"{buildConfiguration}_win-x64");
      Assert.Equal(0, DotnetCli($"build \"{wpfProjectPath}\"", out string output, out string error));
      string assemblyLocation = Directory.GetFiles(testBinaryPath, "coverlet.tests.projectsample.wpf8.selfcontained.dll", SearchOption.AllDirectories).First();

      var mockLogger = new Mock<ILogger>();
      var resolver = new NetCoreSharedFrameworkResolver(assemblyLocation, mockLogger.Object);
      var compilationLibrary = new CompilationLibrary(
          "package",
          "System.Drawing",
          "0.0.0.0",
          "sha512-not-relevant",
          Enumerable.Empty<string>(),
          Enumerable.Empty<Dependency>(),
          true);

      var assemblies = new List<string>();
      Assert.True(resolver.TryResolveAssemblyPaths(compilationLibrary, assemblies),
          "sample assembly shall be resolved");
      Assert.NotEmpty(assemblies);
    }
  }
}
