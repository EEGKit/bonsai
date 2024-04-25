﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NuGet.Versioning;

namespace Bonsai.Configuration
{
    public readonly struct ScriptExtensionsProjectMetadata
    {
        private readonly XDocument projectDocument;

        public bool Exists => projectDocument is not null;

        const string OuterIndent = "  ";
        const string InnerIndent = "    ";
        const string ItemGroupElement = "ItemGroup";
        const string PackageReferenceElement = "PackageReference";
        const string PackageIncludeAttribute = "Include";
        const string PackageVersionAttribute = "Version";
        const string UseWindowsFormsElement = "UseWindowsForms";
        const string AllowUnsafeBlocksElement = "AllowUnsafeBlocks";

        const string ProjectFileTemplate = @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net472</TargetFramework>
  </PropertyGroup>

</Project>";

        internal ScriptExtensionsProjectMetadata(XDocument projectDocument)
        {
            Debug.Assert(projectDocument is null || projectDocument.Root is not null);
            this.projectDocument = projectDocument;
        }

        private XElement GetProperty(string key)
            => projectDocument?.XPathSelectElement($"/Project/PropertyGroup/{key}");

        private bool GetBoolProperty(string key)
            => string.Equals(GetProperty(key)?.Value, "true", StringComparison.InvariantCultureIgnoreCase);

        public bool AllowUnsafeBlocks => GetBoolProperty(AllowUnsafeBlocksElement);

        public IEnumerable<string> GetAssemblyReferences()
        {
            yield return "System.dll";
            yield return "System.Core.dll";
            yield return "System.Drawing.dll";
            yield return "System.Numerics.dll";
            yield return "System.Reactive.Linq.dll";
            yield return "System.Runtime.Serialization.dll";
            yield return "System.Xml.dll";
            yield return "Bonsai.Core.dll";
            yield return "Microsoft.CSharp.dll";
            yield return "netstandard.dll";

            if (GetBoolProperty(UseWindowsFormsElement))
                yield return "System.Windows.Forms.dll";
        }

        public IEnumerable<string> GetPackageReferences()
        {
            if (!Exists)
                return Enumerable.Empty<string>();

            return from element in projectDocument.Descendants(XName.Get(PackageReferenceElement))
                   let id = element.Attribute(PackageIncludeAttribute)
                   where id != null
                   select id.Value;
        }

        public ScriptExtensionsProjectMetadata AddPackageReferences(IEnumerable<PackageReference> packageReferences)
        {
            var root = projectDocument?.Root;
            root ??= XElement.Parse(ProjectFileTemplate, LoadOptions.PreserveWhitespace);

            var projectReferences = root.Descendants(PackageReferenceElement).ToArray();
            var lastReference = projectReferences.LastOrDefault();

            foreach (var reference in packageReferences)
            {
                var includeAttribute = new XAttribute(PackageIncludeAttribute, reference.Id);
                var versionAttribute = new XAttribute(PackageVersionAttribute, reference.Version);
                var existingReference = (from element in projectReferences
                                         let id = element.Attribute(PackageIncludeAttribute)
                                         where id != null && id.Value == reference.Id
                                         select element).FirstOrDefault();
                if (existingReference != null)
                {
                    var version = existingReference.Attribute(PackageVersionAttribute);
                    if (version == null)
                        existingReference.Add(versionAttribute);
                    else if (NuGetVersion.Parse(version.Value) < NuGetVersion.Parse(reference.Version))
                        version.SetValue(reference.Version);

                    continue;
                }

                var referenceElement = new XElement(PackageReferenceElement, includeAttribute, versionAttribute);
                if (lastReference == null)
                {
                    var itemGroup = new XElement(ItemGroupElement);
                    itemGroup.Add(Environment.NewLine + InnerIndent);
                    itemGroup.Add(referenceElement);
                    itemGroup.Add(Environment.NewLine + OuterIndent);

                    root.Add(OuterIndent);
                    root.Add(itemGroup);
                    root.Add(Environment.NewLine);
                    root.Add(Environment.NewLine);
                }
                else
                {
                    lastReference.AddAfterSelf(referenceElement);
                    if (lastReference.PreviousNode != null && lastReference.PreviousNode.NodeType >= XmlNodeType.Text)
                        lastReference.AddAfterSelf(lastReference.PreviousNode);
                }
                lastReference = referenceElement;
            }

            return new ScriptExtensionsProjectMetadata(new XDocument(root));
        }

        public string GetProjectXml()
            => Exists ? projectDocument.Root.ToString(SaveOptions.DisableFormatting) : ProjectFileTemplate;
    }
}
