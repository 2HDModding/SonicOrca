// Decompiled with JetBrains decompiler
// Type: SonicOrca.IniConfiguration
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SonicOrca
{

    public class IniConfiguration
    {
      private readonly List<string> _lines = new List<string>();
      private readonly Dictionary<string, IniConfiguration.Section> _sections = new Dictionary<string, IniConfiguration.Section>();
      private string _path;

      public IniConfiguration() => this._sections[string.Empty] = new IniConfiguration.Section();

      public IniConfiguration(string path)
        : this()
      {
        this._path = path;
        using (StreamReader streamReader = new StreamReader((Stream) new FileStream(path, FileMode.Open, FileAccess.Read)))
        {
          string str;
          while ((str = streamReader.ReadLine()) != null)
            this._lines.Add(str);
        }
        this.Parse();
      }

      public void Save() => this.Save(this._path);

      public void Save(string path)
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) new FileStream(path, FileMode.Create, FileAccess.Write)))
        {
          foreach (string line in this._lines)
            streamWriter.WriteLine(line);
        }
      }

      public string this[string sectionName, string propertyName]
      {
        get => this.GetProperty(sectionName, propertyName);
        set => this.SetProperty(sectionName, propertyName, value);
      }

      public bool PropertyExists(string sectionName, string propertyName)
      {
        return this._sections.ContainsKey(sectionName) && this._sections[sectionName].Properties.ContainsKey(propertyName);
      }

      public string GetProperty(string sectionName, string propertyName, string defaultValue = null)
      {
        if (!this._sections.ContainsKey(sectionName))
          return defaultValue;
        IDictionary<string, IniConfiguration.Property> properties = this._sections[sectionName].Properties;
        return !properties.ContainsKey(propertyName) ? defaultValue : properties[propertyName].Value;
      }

      public bool GetPropertyBoolean(string sectionName, string propertyName, bool defaultValue = false)
      {
        string property = this.GetProperty(sectionName, propertyName);
        bool result;
        return property == null || !bool.TryParse(property, out result) ? defaultValue : result;
      }

      public double GetPropertyDouble(string sectionName, string propertyName, double defaultValue = 0.0)
      {
        if (!this._sections.ContainsKey(sectionName))
          return defaultValue;
        IDictionary<string, IniConfiguration.Property> properties = this._sections[sectionName].Properties;
        double result;
        return !properties.ContainsKey(propertyName) || !double.TryParse(properties[propertyName].Value, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? defaultValue : result;
      }

      public void SetProperty(string sectionName, string propertyName, string value)
      {
        IniConfiguration.Section orAddSection = this.GetOrAddSection(sectionName);
        if (orAddSection.Properties.ContainsKey(propertyName))
        {
          IniConfiguration.Property property = orAddSection.Properties[propertyName];
          property.Value = value;
          this._lines[property.Lines.Last<int>()] = this.GetPropertySetValueLine(propertyName, value);
        }
        else
        {
          IniConfiguration.Property property;
          orAddSection.Properties[propertyName] = property = new IniConfiguration.Property(propertyName);
          property.Value = value;
          if (orAddSection.Lines.Count > 0)
          {
            int index = orAddSection.Lines.Last<int>() + 1;
            this._lines.Insert(index, this.GetPropertySetValueLine(propertyName, value));
            property.Lines.Add(index);
          }
          else
          {
            if (this._lines.Count > 0 && !string.IsNullOrWhiteSpace(this._lines.Last<string>()))
              this._lines.Add(string.Empty);
            this._lines.Add(this.GetPropertySetValueLine(propertyName, value));
            property.Lines.Add(this._lines.Count - 1);
          }
        }
      }

      private string GetPropertySetValueLine(string propertyName, string value)
      {
        return $"{propertyName} = {value}";
      }

      private void InsertLine(int index, string line)
      {
        foreach (IniConfiguration.Section section in this._sections.Values)
        {
          foreach (IniConfiguration.Property property in (IEnumerable<IniConfiguration.Property>) section.Properties.Values)
          {
            for (int index1 = 0; index1 < property.Lines.Count; ++index1)
            {
              if (property.Lines[index1] <= index)
                property.Lines[index1]++;
            }
          }
        }
        this._lines.Insert(index, line);
      }

      private IniConfiguration.Section GetOrAddSection(string sectionName)
      {
        if (this._sections.ContainsKey(sectionName))
          return this._sections[sectionName];
        IniConfiguration.Section section = new IniConfiguration.Section(sectionName);
        if (this._lines.Count > 0 && !string.IsNullOrWhiteSpace(this._lines.Last<string>()))
          this._lines.Add(string.Empty);
        this._lines.Add($"[{sectionName}]");
        section.Lines.Add(this._lines.Count - 1);
        return this._sections[sectionName] = section;
      }

      private IniConfiguration.Property GetOrAddProperty(string sectionName, string propertyName)
      {
        IDictionary<string, IniConfiguration.Property> properties = this._sections[sectionName].Properties;
        return !properties.ContainsKey(propertyName) ? (properties[propertyName] = new IniConfiguration.Property(propertyName)) : properties[propertyName];
      }

      private void Parse()
      {
        string empty = string.Empty;
        for (int index = 0; index < this._lines.Count; ++index)
        {
          try
          {
            this.Parse(index, this._lines[index], ref empty);
          }
          catch
          {
          }
        }
      }

      private void Parse(int lineIndex, string line, ref string currentSectionName)
      {
        StringReader reader = new StringReader(line);
        reader.SkipWhitespace();
        char c;
        if (!reader.TryRead(out c))
          return;
        switch (c)
        {
          case '#':
            break;
          case ';':
            break;
          case '[':
            StringBuilder stringBuilder1 = new StringBuilder();
            while (reader.TryRead(out c) && c != ']')
              stringBuilder1.Append(c);
            if (c != ']')
              throw new Exception("Invalid section name");
            currentSectionName = stringBuilder1.ToString().Trim();
            IniConfiguration.Section section;
            if (!this._sections.TryGetValue(currentSectionName, out section))
            {
              section = new IniConfiguration.Section(currentSectionName);
              this._sections.Add(currentSectionName, section);
            }
            section.Lines.Add(lineIndex);
            break;
          default:
            StringBuilder stringBuilder2 = new StringBuilder();
            stringBuilder2.Append(c);
            while (reader.TryRead(out c) && c != '=' && c != ':')
              stringBuilder2.Append(c);
            if (c != '=' && c != ':')
              throw new Exception("Expected = or : after property name");
            string propertyName = stringBuilder2.ToString().Trim();
            string str = this.ReadPropertyValue((TextReader) reader);
            IniConfiguration.Property orAddProperty = this.GetOrAddProperty(currentSectionName, propertyName);
            orAddProperty.Value = str;
            orAddProperty.Lines.Add(lineIndex);
            break;
        }
      }

      private string ReadPropertyValue(TextReader reader)
      {
        reader.SkipWhitespace();
        StringBuilder stringBuilder = new StringBuilder();
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        char c;
        if (!reader.TryRead(out c))
          return (string) null;
        switch (c)
        {
          case '"':
            flag2 = flag3 = true;
            break;
          case '\'':
            flag1 = flag3 = true;
            break;
          default:
            stringBuilder.Append(c);
            break;
        }
        while (reader.TryRead(out c))
        {
          switch (c)
          {
            case '#':
            case ';':
              if (!flag3)
                goto label_15;
              break;
            case '\\':
              stringBuilder.Append(this.ReadEscapedCharacter(reader));
              continue;
          }
          if (c == '\'' & flag1)
          {
            flag1 = false;
            break;
          }
          if (c == '"' & flag2)
          {
            flag2 = false;
            break;
          }
          stringBuilder.Append(c);
        }
    label_15:
        if (flag1 | flag2)
          throw new Exception("No end quote(s)");
        return !flag3 ? stringBuilder.ToString().TrimEnd() : stringBuilder.ToString();
      }

      private char ReadEscapedCharacter(TextReader reader)
      {
        char c;
        if (!reader.TryRead(out c))
          throw new Exception("Expected escape character");
        switch (c)
        {
          case '0':
            return char.MinValue;
          case 'n':
            return '\n';
          case 'r':
            return '\r';
          case 't':
            return '\t';
          case 'x':
            char[] buffer = new char[4];
            return reader.ReadBlock(buffer, 0, 4) == 4 ? (char) int.Parse(new string(buffer), NumberStyles.AllowHexSpecifier) : throw new Exception("Wrong format for escaped unicode character.");
          default:
            return c;
        }
      }

      private class Section
      {
        private readonly string _name;
        private readonly List<int> _lines = new List<int>();
        private readonly Dictionary<string, IniConfiguration.Property> _properties = new Dictionary<string, IniConfiguration.Property>();

        public IList<int> Lines => (IList<int>) this._lines;

        public IDictionary<string, IniConfiguration.Property> Properties
        {
          get => (IDictionary<string, IniConfiguration.Property>) this._properties;
        }

        public Section()
          : this(string.Empty)
        {
        }

        public Section(string name) => this._name = name;
      }

      private class Property
      {
        private readonly string _name;
        private readonly List<int> _lines = new List<int>();

        public string Value { get; set; }

        public IList<int> Lines => (IList<int>) this._lines;

        public Property(string name) => this._name = name;
      }
    }
}
