﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("NuGetGallery")]
[assembly: InternalsVisibleTo("NuGetGallery.Facts")]

[assembly: AssemblyCompany(".NET Foundation")]
[assembly: AssemblyProduct("NuGet Services")]
[assembly: AssemblyCopyright("\x00a9 .NET Foundation. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#if !PORTABLE
[assembly: ComVisible(false)]
#endif

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: CLSCompliant(false)]
[assembly: NeutralResourcesLanguage("en-us")]

// The build will automatically inject the following attributes:
// AssemblyVersion, AssemblyFileVersion, AssemblyInformationalVersion, AssemblyMetadata (for Branch, CommitId, and BuildDateUtc)

[assembly: AssemblyMetadata("RepositoryUrl", "https://www.github.com/NuGet/NuGetGallery")]

[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0-HEAD")]
#if !PORTABLE
[assembly: AssemblyMetadata("Branch", "HEAD")]
[assembly: AssemblyMetadata("CommitId", "82d98a7")]
[assembly: AssemblyMetadata("BuildDateUtc", "09/24/2016 12:00:05 +00:00")]
#endif

[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0-master")]
#if !PORTABLE
[assembly: AssemblyMetadata("Branch", "master")]
[assembly: AssemblyMetadata("CommitId", "0c64581")]
[assembly: AssemblyMetadata("BuildDateUtc", "09/24/2016 12:24:04 +00:00")]
#endif
