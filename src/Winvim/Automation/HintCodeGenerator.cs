namespace Winvim.Automation;

/// <summary>
/// Generates short, fixed-width, prefix-free hint codes (e.g. "aa", "as", "ad", ...) — one
/// per element to label. Fixed width keeps the prefix-free property trivially true: no code
/// can ever be a prefix of another, so typing any code unambiguously narrows to one match.
/// </summary>
public static class HintCodeGenerator
{
    public const string DefaultAlphabet = "asdfghjkl";

    public static IReadOnlyList<string> Generate(int count, string alphabet = DefaultAlphabet)
    {
        if (count <= 0) return Array.Empty<string>();

        int width = 1;
        while (Math.Pow(alphabet.Length, width) < count)
            width++;

        var codes = new List<string>(count);
        var buffer = new char[width];
        GenerateRecursive(alphabet, buffer, 0, width, codes, count);
        return codes;
    }

    private static void GenerateRecursive(string alphabet, char[] buffer, int position, int width, List<string> codes, int limit)
    {
        if (codes.Count >= limit) return;

        if (position == width)
        {
            codes.Add(new string(buffer));
            return;
        }

        foreach (char c in alphabet)
        {
            if (codes.Count >= limit) return;
            buffer[position] = c;
            GenerateRecursive(alphabet, buffer, position + 1, width, codes, limit);
        }
    }
}
