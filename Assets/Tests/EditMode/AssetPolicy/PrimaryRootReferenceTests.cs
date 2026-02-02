using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace ProjectAction.AssetPolicy.Tests
{
    public sealed class PrimaryRootReferenceTests
    {
        private const string PRIMARY_ROOT_PREFIX = "Assets/ProjectContents/PrimaryRoot/";

        [Test]
        public void NoAssetsReferencePrimaryRoot()
        {
            var assetPaths = FindTargetAssets();
            var violations = assetPaths
                .Select(FindViolationsForAsset)
                .Where(HasViolations)
                .ToList();

            if (!violations.Any())
            {
                Assert.Pass("No assets reference PrimaryRoot.");
            }

            var message = string.Join(
                Environment.NewLine,
                violations.Select(FormatViolation));

            Assert.Fail($"PrimaryRoot references found:{Environment.NewLine}{message}");
        }

        private static IEnumerable<string> FindTargetAssets()
        {
            return AssetDatabase.FindAssets("t:Prefab t:Scene t:ScriptableObject")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(IsValidAssetPath)
                .Where(IsNotPrimaryRootAsset)
                .Distinct();
        }

        private static Violation FindViolationsForAsset(string assetPath)
        {
            var dependencies = AssetDatabase.GetDependencies(assetPath, true);
            var primaryReferences = dependencies
                .Where(IsPrimaryRootPath)
                .Distinct()
                .ToArray();

            return new Violation(assetPath, primaryReferences);
        }

        private static bool HasViolations(Violation violation)
        {
            return violation.PrimaryReferences.Length > 0;
        }

        private static string FormatViolation(Violation violation)
        {
            var references = string.Join(", ", violation.PrimaryReferences);
            return $"{violation.AssetPath} -> {references}";
        }

        private static bool IsValidAssetPath(string assetPath)
        {
            return !string.IsNullOrWhiteSpace(assetPath);
        }

        private static bool IsPrimaryRootPath(string assetPath)
        {
            return assetPath.StartsWith(PRIMARY_ROOT_PREFIX, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsNotPrimaryRootAsset(string assetPath)
        {
            return !IsPrimaryRootPath(assetPath);
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
#endif
