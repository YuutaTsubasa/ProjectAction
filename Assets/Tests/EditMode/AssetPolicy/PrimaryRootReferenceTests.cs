using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using UnityEditor;

namespace ProjectAction.AssetPolicy.Tests
{
    public sealed class PrimaryRootReferenceTests
    {
        private const string PRIMARY_ROOT_PREFIX = "Assets/ProjectContents/PrimaryRoot/";

        [Test]
        public void NoAssetsReferencePrimaryRoot()
        {
            var assetPaths = _FindTargetAssets();
            var violations = assetPaths
                .Select(_FindViolationsForAsset)
                .Where(_HasViolations)
                .ToList();

            if (!violations.Any())
            {
                Assert.Pass("No assets reference PrimaryRoot.");
            }

            var message = string.Join(
                Environment.NewLine,
                violations.Select(_FormatViolation));

            Assert.Fail($"PrimaryRoot references found:{Environment.NewLine}{message}");
        }

        private static IEnumerable<string> _FindTargetAssets()
        {
            return AssetDatabase.FindAssets("t:Prefab t:Scene t:ScriptableObject")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(_IsValidAssetPath)
                .Distinct();
        }

        private static Violation _FindViolationsForAsset(string assetPath)
        {
            var dependencies = AssetDatabase.GetDependencies(assetPath, true);
            var primaryReferences = dependencies
                .Where(_IsPrimaryRootPath)
                .Distinct()
                .ToArray();

            return new Violation(assetPath, primaryReferences);
        }

        private static bool _HasViolations(Violation violation)
        {
            return violation.PrimaryReferences.Length > 0;
        }

        private static string _FormatViolation(Violation violation)
        {
            var references = string.Join(", ", violation.PrimaryReferences);
            return $"{violation.AssetPath} -> {references}";
        }

        private static bool _IsValidAssetPath(string assetPath)
        {
            return !string.IsNullOrWhiteSpace(assetPath);
        }

        private static bool _IsPrimaryRootPath(string assetPath)
        {
            return assetPath.StartsWith(PRIMARY_ROOT_PREFIX, StringComparison.OrdinalIgnoreCase);
        }

        private sealed class Violation
        {
            public Violation(string assetPath, string[] primaryReferences)
            {
                AssetPath = assetPath;
                PrimaryReferences = primaryReferences;
            }

            public string AssetPath { get; }
            public string[] PrimaryReferences { get; }
        }
    }
}
