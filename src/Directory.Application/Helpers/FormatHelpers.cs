namespace Directory.Application.Helpers;

public static class FormatHelpers
{
    public static string FormatPhone(string? phone)
    {
        if (string.IsNullOrEmpty(phone))
            return string.Empty;

        var digits = new string(phone.Where(char.IsDigit).ToArray());

        return digits.Length switch
        {
            10 => $"({digits[..3]}) {digits[3..6]}-{digits[6..]}",
            7 => $"{digits[..3]}-{digits[3..]}",
            _ => digits.Length > 6 ? digits : phone
        };
    }

    public static string CleanPhone(string? phone)
    {
        if (string.IsNullOrEmpty(phone))
            return string.Empty;

        var digits = new string(phone.Where(char.IsDigit).ToArray());
        return digits.Length > 10 ? digits[..10] : digits;
    }

    public static string CleanZip(string? zip)
    {
        if (string.IsNullOrEmpty(zip))
            return string.Empty;

        var digits = new string(zip.Where(char.IsDigit).ToArray());
        return digits.Length > 5 ? digits[..5] : digits;
    }

    public static string FormatAddress(string? address1, string? address2, string? city, string? stateName, string? zip)
    {
        var lines = new List<string>();

        if (!string.IsNullOrEmpty(address1))
            lines.Add(address1);
        if (!string.IsNullOrEmpty(address2))
            lines.Add(address2);

        var cityStateZip = new List<string>();
        if (!string.IsNullOrEmpty(city))
            cityStateZip.Add(city);

        var csz = string.Empty;
        if (cityStateZip.Count > 0)
            csz = string.Join(", ", cityStateZip);
        if (!string.IsNullOrEmpty(stateName))
            csz += (csz.Length > 0 ? ", " : "") + stateName;
        if (!string.IsNullOrEmpty(zip))
            csz += (csz.Length > 0 ? " " : "") + zip;

        if (!string.IsNullOrEmpty(csz))
            lines.Add(csz);

        return string.Join("\n", lines);
    }
}
