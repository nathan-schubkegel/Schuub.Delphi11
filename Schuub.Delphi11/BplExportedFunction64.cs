// This is free and unencumbered software released into the public domain under The Unlicense.
// You have complete freedom to do anything you want with the software, for any purpose.
// Please refer to <http://unlicense.org/>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Schuub.Delphi11
{
  public static class BplExportedFunction64
  {
    public static BplExportedFunction Parse(string mangledName)
    {
      if (!mangledName.StartsWith("_Z"))
      {
        throw new InvalidDataException();
      }

      // <name> ::= <nested-name>
      //        ::= <unscoped-name>
      //        ::= <unscoped-template-name> <template-args>
      //        ::= <local-name>	# See Scope Encoding below

      int index = 2;
      var result = new BplExportedFunction
      {
        MangledName = mangledName
      };

      if (mangledName.ElementAtOrDefault(index) != 'N')
      {
        throw new NotImplementedException($"Not yet able to handle character " +
          $"'{mangledName[index]}' at index {index} of mangled name: {mangledName}");
      }
      var substitutions = new List<string>();
      result.DemangledName = ReadNestedName(mangledName, ref index, substitutions);

      BplExportedType parameter = null;
      int startOfMangledParameter = -1;

      void FinishParameter()
      {
        parameter.MangledName = mangledName.Substring(startOfMangledParameter, index - startOfMangledParameter);
        parameter = null;
        startOfMangledParameter = -1;
      }

      while (index < mangledName.Length)
      {
        if (parameter == null)
        {
          parameter = new BplExportedType();
          startOfMangledParameter = index;
          result.Parameters.Add(parameter);
        }

        switch (mangledName[index])
        {
          // <ref-qualifier> ::= R              # & ref-qualifier
          //                 ::= O              # && ref-qualifier
          case 'R':
            parameter.Flags |= BplExportedTypeFlags.Reference;
            index++;
            break;

          // <CV-qualifiers> ::= [r] [V] [K] 	  # restrict (C99), volatile, const
          case 'K':
            parameter.Flags |= BplExportedTypeFlags.Const;
            index++;
            break;

          // <nested-name> ::= N [<CV-qualifiers>] [<ref-qualifier>] <prefix> <unqualified-name> E
          //               ::= N [<CV-qualifiers>] [<ref-qualifier>] <template-prefix> <template-args> E
          case 'N':
            parameter.DemangledName = ReadNestedName(mangledName, ref index, substitutions);
            FinishParameter();
            break;

          case 'i':
            parameter.DemangledName = "integer";
            index++;
            FinishParameter();
            break;

          default:
            throw new NotImplementedException($"Not yet able to handle character " +
              $"'{mangledName[index]}' at index {index} of mangled name: {mangledName}");
        }
      }

      if (parameter != null)
      {
        throw new InvalidDataException();
      }

      return result;
    }

    private static string ReadNestedName(string mangledName, ref int index, List<string> substitutions)
    {
      if (mangledName[index] != 'N')
      {
        throw new ArgumentException();
      }
      index++;

      // <nested-name> ::= N [<CV-qualifiers>] [<ref-qualifier>] <prefix> <unqualified-name> E
      //               ::= N [<CV-qualifiers>] [<ref-qualifier>] <template-prefix> <template-args> E
      //

      //
      // <prefix> ::= <unqualified-name>                 # global class or namespace
      //          ::= <prefix> <unqualified-name>        # nested class or namespace
      //          ::= <template-prefix> <template-args>  # class template specialization
      //          ::= <closure-prefix>                   # initializer of a variable or data member
      //          ::= <template-param>                   # template type parameter
      //          ::= <decltype>                         # decltype qualifier
      //          ::= <substitution>
      //
      // <unqualified-name> ::= <operator-name> [<abi-tags>]
      //                    ::= <ctor-dtor-name>
      //                    ::= <source-name>
      //                    ::= <unnamed-type-name>
      //                    ::= DC <source-name>+ E      # structured binding declaration
      //
      // <source-name> ::= <positive length number> <identifier>
      // <identifier>  ::= whatever the heck characters make up the name

      StringBuilder nameBuilder = new StringBuilder();

      void AddSubstitution()
      {
        var result = nameBuilder.ToString();
        if (!substitutions.Contains(result))
        {
          substitutions.Add(result);
        }
      }

      void AddToNameBuilder(string whatever)
      {
        if (nameBuilder.Length > 0)
        {
          nameBuilder.Append('.');
        }
        nameBuilder.Append(whatever);
        AddSubstitution();
      }

      while (true)
      {
        if (index >= mangledName.Length)
        {
          throw new InvalidDataException();
        }

        char c = mangledName[index];

        // 'E' means end of the name
        if (c == 'E')
        {
          index++;
          return nameBuilder.ToString();
        }

        if (c == 'S')
        {
          index++;
          string substitution = ReadSubstitution(mangledName, ref index, substitutions);
          AddToNameBuilder(substitution);
          continue;
        }

        if (char.IsDigit(c))
        {
          var sourceName = ReadSourceName(mangledName, ref index);
          AddToNameBuilder(sourceName);
          continue;
        }

        throw new NotImplementedException($"Not yet able to handle character " +
          $"'{c}' at index {index} of mangled name: {mangledName}");
      }
    }

    private static string ReadSourceName(string mangledName, ref int index)
    {
      // <source-name> ::= <positive length number> <identifier>
      //
      // <identifier> ::= a sequence of whatever characters

      // decimal digits for how many characters in the next chunk of the name
      int countOfLengthDigits = 0;
      while (true)
      {
        if (index + countOfLengthDigits >= mangledName.Length)
        {
          throw new InvalidDataException();
        }

        if (char.IsDigit(mangledName[index + countOfLengthDigits]))
        {
          countOfLengthDigits++;
        }
        else
        {
          break;
        }
      }
      if (!int.TryParse(
        mangledName.Substring(index, countOfLengthDigits),
        NumberStyles.Integer,
        CultureInfo.InvariantCulture,
        out int countOfNameLetters))
      {
        throw new InvalidDataException();
      }

      index += countOfLengthDigits;

      if (index + countOfNameLetters >= mangledName.Length)
      {
        throw new InvalidDataException();
      }

      var result = mangledName.Substring(index, countOfNameLetters);
      index += countOfNameLetters;
      return result;
    }

    private static string ReadSubstitution(string mangledName, ref int index, List<string> substitutions)
    {
      //  <substitution> ::= S_            # first substitution
      //                 ::= S <seq-id> _  # second and beyond, starting at number 0
      //
      //        <seq-id> ::= <0-9A-Z>+  # number in base 36, using digits and upper case letters.
      //
      // Substitutable components are numbered left-to-right. 
      // A component is earlier in the substitution dictionary than
      // the structure of which it is a part. All substitutable
      // components are numbered, except those that have already been
      // numbered for substitution. For example:
      //
      // For this function: Ret? N::T<int, int>::mf(N::T<double, double>)
      // "_ZN1N1TIiiE2mfES0_IddE"
      // "_Z N 1N 1T IiiE 2mf E S0_ IddE" (important parts split by spaces) 
      //    
      // the substitutions generated for this name are:
      // "S_" == N (qualifier is less recent than qualified entity)
      // "S0_" == N::T (template-id comes before template)
      // (int is builtin, and isn't considered)
      // "S1_" == N::T<int, int>
      // "S2_" == N::T<double, double>

      if (index >= mangledName.Length)
      {
        throw new InvalidDataException();
      }
      char c = mangledName[index];
      string substitutionDigits = "";
      while (c != '_')
      {
        if (c >= '0' && c <= '9')
        {
          substitutionDigits += c;
        }
        else if (c >= 'A' && c <= 'Z')
        {
          substitutionDigits += c;
        }
        else
        {
          throw new InvalidDataException();
        }
        index++;
        if (index >= mangledName.Length)
        {
          throw new InvalidDataException();
        }
        c = mangledName[index];
      }

      // move on past final '_'
      index++;

      int substitutionIndex = substitutionDigits == "" ? 0 :
        (1 + Base36ToInt(substitutionDigits));

      if (substitutionIndex >= substitutions.Count)
      {
        throw new InvalidDataException();
      }
      return substitutions[substitutionIndex];
    }

    private static int Base36ToInt(string digits)
    {
      // <seq-id> ::= <0-9A-Z>+
      // A <seq-id> is a sequence number in base 36, using digits and upper case letters.
      int result = 0;
      foreach (char c in digits)
      {
        result *= 36;
        if (c >= '0' && c <= '9')
        {
          result += (c - '0');
        }
        else if (c >= 'A' && c <= 'Z')
        {
          result += 10 + (c - 'A');
        }
        else
        {
          throw new InvalidDataException();
        }
      }
      return result;
    }
  }
}
