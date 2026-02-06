namespace JobPulse.Core.Services;

/// <summary>
/// Technology groups for keyword expansion and filtering.
/// Each group contains related technologies that are often used together.
/// Any keyword from a group will match the entire group during filtering.
/// </summary>
public static class TechGroups
{
    private static readonly string[][] Groups =
    {
        // C# / .NET ecosystem
        new[] { "c#", "csharp", ".net", "dotnet", "asp.net", "blazor", "maui", "xamarin", "ef", "entity framework" },

        // JavaScript / TypeScript ecosystem
        new[] { "javascript", "js", "typescript", "ts", "node", "nodejs", "node.js", "react", "vue", "angular", "next.js", "nextjs", "express" },

        // Python ecosystem
        new[] { "python", "django", "flask", "fastapi", "pandas", "numpy", "pytorch", "tensorflow" },

        // Java ecosystem
        new[] { "java", "spring", "springboot", "spring boot", "kotlin", "gradle", "maven" },

        // PHP ecosystem
        new[] { "php", "laravel", "symfony", "wordpress" },

        // Ruby ecosystem
        new[] { "ruby", "rails", "ruby on rails", "ror" },

        // Go
        new[] { "go", "golang" },

        // Rust
        new[] { "rust", "cargo" },

        // Mobile
        new[] { "swift", "ios", "swiftui", "uikit" },
        new[] { "android", "kotlin", "jetpack compose" },
        new[] { "flutter", "dart" },
        new[] { "react native", "expo" },

        // DevOps / Cloud
        new[] { "aws", "amazon web services", "ec2", "s3", "lambda" },
        new[] { "azure", "microsoft azure" },
        new[] { "gcp", "google cloud", "google cloud platform" },
        new[] { "docker", "kubernetes", "k8s", "helm" },
        new[] { "terraform", "ansible", "pulumi" },

        // Databases
        new[] { "sql", "mysql", "postgresql", "postgres", "mssql", "sql server" },
        new[] { "mongodb", "mongo" },
        new[] { "redis", "memcached" },

        // Data / ML
        new[] { "machine learning", "ml", "deep learning", "ai", "artificial intelligence" },
        new[] { "data science", "data scientist", "data analyst", "data analytics" },
    };

    /// <summary>
    /// Builds a set of filter keywords based on user input.
    /// Parses input, finds matching tech groups, returns all related aliases.
    /// </summary>
    /// <param name="keywords">User's search keywords (e.g., "C# developer Vancouver")</param>
    /// <returns>Set of lowercase aliases to filter by, or empty if no tech keywords found</returns>
    public static HashSet<string> BuildFilterSet(string? keywords)
    {
        if (string.IsNullOrWhiteSpace(keywords))
            return new HashSet<string>();

        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var lowerKeywords = keywords.ToLower();

        foreach (var group in Groups)
        {
            // Check if any term from the group appears in user's keywords
            foreach (var term in group)
            {
                if (lowerKeywords.Contains(term))
                {
                    // Add all terms from this group to filter set
                    foreach (var alias in group)
                    {
                        result.Add(alias);
                    }
                    break; // Move to next group
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Checks if job content matches any of the filter aliases.
    /// </summary>
    /// <param name="filterSet">Set of aliases from BuildFilterSet</param>
    /// <param name="title">Job title</param>
    /// <param name="description">Job description</param>
    /// <returns>True if at least one alias found in title or description</returns>
    public static bool MatchesFilter(HashSet<string> filterSet, string? title, string? description)
    {
        if (filterSet.Count == 0)
            return true; // No tech filter = accept all

        var combined = $"{title} {description}".ToLower();
        return filterSet.Any(alias => combined.Contains(alias));
    }
}
