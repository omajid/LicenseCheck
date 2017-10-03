using System;

namespace LicenseCheck
{

    public class LicenseChecker
    {

        private static string[] LICENSES = {
            // Apache
            "Copyright (c) Microsoft. All Rights Reserved. Licensedf under the Apache License, Version 2.0. See License.txt in the project root for license information.",
            "Copyright (c) Microsoft. All Rights Reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.",
            "Copyright(c) Microsoft.All Rights Reserved.Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.",
            "Copyright (c) Microsoft Corporation. All Rights Reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.",
            "Copyright (c) .NET Foundation. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.",
            "Copyright (c) Microsoft. All Rights Reserved. Licensed under the Apache License, Version 9.0. See License.txt in the project root for license information.",

            // BSD
            "! Modernizr v2.8.3 www.modernizr.com  Copyright (c) Faruk Ates, Paul Irish, Alex Sexton Available under the BSD and MIT licenses: www.modernizr.com/license/",
            "* A JavaScript implementation of the JSON-LD API.  @author Dave Longley  BSD 3-Clause License Copyright (c) 2011-2013 Digital Bazaar, Inc. All rights reserved.",

            // MIT
            "! Bootstrap v3.3.4 (http://getbootstrap.com) Copyright 2011-2015 Twitter, Inc. Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)",
            "! Bootstrap v3.3.7 (http://getbootstrap.com) Copyright 2011-2016 Twitter, Inc. Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)",
            "! Bootstrap v3.3.7 (http://getbootstrap.com) Copyright 2011-2016 Twitter, Inc. Licensed under the MIT license",
            "Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information",
            "Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.",
            "Copyright(c) Microsoft Corporation.All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.",
            "Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license info",
            "Copyright(c) .NET Foundation and contributors.All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.",
            "Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information",
            "! jQuery JavaScript Library v1.4.1 http://jquery.com/  Copyright 2010, John Resig  Includes Sizzle.js http://sizzlejs.com/ Copyright 2010, The Dojo Foundation",
            "! jQuery JavaScript Library v2.2.0 http://jquery.com/  Includes Sizzle.js http://sizzlejs.com/  Copyright jQuery Foundation and other contributors Released under the MIT license",
            "! jQuery JavaScript Library v2.1.3 http://jquery.com/  Includes Sizzle.js http://sizzlejs.com/  Copyright 2005, 2014 jQuery Foundation, Inc. and other contributors Released under the MIT license http://jquery.org/license",
            "! jQuery v2.1.3 | (c) 2005, 2014 jQuery Foundation, Inc. | jquery.org/license",
            "! jQuery v2.2.0 | (c) jQuery Foundation | jquery.org/license",
            "! jQuery Validation Plugin v1.13.1  http://jqueryvalidation.org/  Copyright (c) 2014 Jörn Zaefferer Released under the MIT license",
            "! jQuery Validation Plugin - v1.13.1 - 10/14/2014 http://jqueryvalidation.org/ Copyright (c) 2014 Jörn Zaefferer; Licensed MIT",
            "! jQuery Validation Plugin - v1.14.0 - 6/30/2015 http://jqueryvalidation.org/ Copyright (c) 2015 Jörn Zaefferer; Licensed MIT",
            "! matchMedia() polyfill - Test a CSS media type/query in JS. Authors & copyright (c) 2012: Scott Jehl, Paul Irish, Nicholas Zakas. Dual MIT/BSD license",
            "! jQuery Validation Plugin v1.14.0  http://jqueryvalidation.org/  Copyright (c) 2015 Jörn Zaefferer Released under the MIT license",
            "Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE file in the project root for more information",
            "Licensed to the .NET Foundation under one or more agreements.--- The .NET Foundation licenses this file to you under the MIT license. See the LICENSE file in the project root for more information.",
            "Licensed to the.NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE file in the project root for more information.",
            "Licensed under the MIT license. See LICENSE file in the project root for full license information.",
            "! Respond.js v1.4.2: min/max-width media query polyfill * Copyright 2013 Scott Jehl Licensed under https://github.com/scottjehl/Respond/blob/master/LICENSE-MIT",
            "The .NET Foundation licenses this file to you under the MIT license. See the LICENSE file in the project root for more information.",

            // Unknown
            "Copyright (c) Microsoft. All rights reserved.", // TODO too generic with a StartsWith clause
            "Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.",
            "----------------------------------------------------------------------- <copyright file=\"{0}\" company=\"Microsoft\"> Copyright (c) Microsoft Corporation. All rights reserved. </copyright>",
            "------------------------------------------------------------------------------ <copyright file=\"{0}\" company=\"Microsoft\"> Copyright (c) Microsoft Corporation. All rights reserved. </copyright>",
            "------------------------------------------------------------------------------ <copyright file=\"{0}\" company=\"Microsoft\"> Copyright (c) Microsoft Corporation. All rights reserved. </copyright>",
            "<copyright file=\"{0}\" company=\"Microsoft\"> Copyright © Microsoft. All Rights Reserved. </copyright>",
            "----------------------------------------------------------------------- <copyright file=\"{0}\" company=\"Microsoft\"> Copyright © Microsoft. All Rights Reserved. </copyright>",
            "------------------------------------------------------------------------------ <copyright file=\"{0}\" company=\"Microsoft\"> Copyright © Microsoft. All Rights Reserved. </copyright>",
            "jQuery validation plug-in 1.6  http://bassistance.de/jquery-plugins/jquery-plugin-validation/ http://docs.jquery.com/Plugins/Validation  Copyright (c) 2006 - 2008 Jörn Zaefferer",
            "Licensed to the .NET Foundation under one or more agreements. See the LICENSE file in the project root for more information.",
        };

        private static string[] NOT_LICENSES =
        {
            "------------------------------------------------------------------------------ <auto-generated> This code was generated by a tool.",
            "------------------------------------------------------------------------------ This code was generated by a tool",
            "Definition of syntax model. Generated by a tool from",
            "<auto-generated />",
            "---------------------------------------------------------------------------- <autogenerated> This code was generated by a tool.",
            "This file was autogenerated by running the script in this directory",
            "------------------------------------------------------------------------------ <autogenerated> This code was generated by a tool."
        };

        public LicenseCheckResult Check(FilePath file)
        {
            string fileName = file.GetFileName();

            FileType fileType = new FileTypeIdentifier().Identify(file);

            var licenseHeaderExtractor = new LicenseHeaderExtractor();

            string licenseHeader = licenseHeaderExtractor.Extract(file, fileType);

            if (fileType == FileType.Unknown && (file.ContainsPath("/cpp/") ||
                                                 file.ContainsPath("/inc/clr_std/")))
            {
                licenseHeader = licenseHeaderExtractor.Extract(file, FileType.C);
            }

            if (fileType == FileType.Unknown)
            {
                return new LicenseCheckResult() { License = LicenseType.DontKnowHowToParseThisFile, File = file, IdentifiedType = FileType.Unknown , OptionalDetails = null};
            }

            licenseHeader = RemoveBogusLicenseHeader(licenseHeader, file);

            if (string.IsNullOrEmpty(licenseHeader))
            {
                // stdout.WriteLine("Missing license header in {0}", file);
                return new LicenseCheckResult() { License = LicenseType.NoLicense, File = file, IdentifiedType = fileType, OptionalDetails = null};
            }
            else
            { 
                bool found = false;
                foreach (var license in LICENSES)
                {
                    var licenseText = String.Format(license, file.GetFileName());
                    //stdout.WriteLine(licenseText);
                    if (licenseHeader.StartsWith(licenseText))
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    return new LicenseCheckResult() { License = LicenseType.ValidLicense, File = file, IdentifiedType = fileType, OptionalDetails = null};
                }
                else
                {
                    return new LicenseCheckResult() { License = LicenseType.UnknownLicense, File = file, IdentifiedType = fileType, OptionalDetails = licenseHeader};
                }
            }
        }

        private string RemoveBogusLicenseHeader(string licenseHeader, FilePath filePath)
        {
            if (String.IsNullOrEmpty(licenseHeader))
            {
                return null;
            }

            foreach (var license in NOT_LICENSES)
            {
                string text = String.Format(license, filePath.GetFileName());
                if (licenseHeader.StartsWith(text))
                {
                    return null;
                }
            }
            return licenseHeader;
        }

    }

}
