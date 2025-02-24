﻿#nullable enable

using System.ComponentModel;
using System.Globalization;

namespace DotNetProjectFile.Conversion;

/// <summary>Implements a <see cref="TypeConverter"/> for <see cref="IOFile"/>.</summary>
internal sealed class IOFileTypeConverter : TypeConverter
{
    /// <inheritdoc />
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    /// <inheritdoc />
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
        => value is null || value is string
            ? IOFile.Parse(value as string)
            : base.ConvertFrom(context, culture, value);
}
