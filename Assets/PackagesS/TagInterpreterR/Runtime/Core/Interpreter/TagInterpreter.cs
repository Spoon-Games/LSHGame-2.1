using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace TagInterpreterR
{
    public static class TagInterpreter
    {
        private static Dictionary<string, TagTypeInfo> tagTypes;

        static TagInterpreter()
        {
            tagTypes = TagTypeLoader.LoadTagTypes();
        }

        #region LoadTags

        public static TagChain LoadTags(string s)
        {
            Token[] tokens = TokenBuilder.BuildTokens(s);

            //DebugTokens(tokens);

            TagChain tagChain = new TagChain();

            for (int i = 0; i < tokens.Length; i++)
            {
                Token token = tokens[i];

                if (token.Is("<"))
                {
                    for (int j = i; j < tokens.Length; j++)
                    {
                        if (tokens[j].Is(">") || tokens[j].Is("/>")) // Missing />
                        {
                            tagChain.Add(CreateTag(tokens, i+1, j));
                            i = j;
                            break;
                        }
                    }
                    continue;
                }else if (token.Is("</"))
                {
                    if (i + 2 >= tokens.Length || !tokens[i + 1].IsDeclaration || !tokens[i + 2].Is(">"))
                        throw new InterpreterException("Wrong syntax at closing statment");
                    tagChain.AddEnd(tokens[i + 1].Value);
                    i += 2;
                }else if (tokens[i].IsString)
                {
                    tagChain.Add(new StringTag(tokens[i].Value));
                }
                else
                    throw new InterpreterException("Wrong syntax " + tokens[i].Value);
            }
            return tagChain;
        }

        private static void DebugTokens(IEnumerable<Token> tokens)
        {
            string debug = "Tokens:\n";
            foreach (var t in tokens)
            {
                debug += t.Value + "\n";
            }
            Debug.Log(debug);
        }

        private static BaseTag CreateTag(Token[] tokens, int start, int end)
        {
            if(start >= end || !tokens[start].IsDeclaration)
            {
                throw new InterpreterException("Wrong tag declaration");
            }

            BaseTag tag;
            string name = tokens[start].Value;
            bool isSingle = tokens[end].Is("/>");

            if (tagTypes.TryGetValue(name, out TagTypeInfo tagInfo)) // Reflection
            {
                if (isSingle && tagInfo.Attribute.Tag == TagAttribute.TagType.Area)
                    throw new InterpreterException("Tag " + name + " is only area");
                if (!isSingle && tagInfo.Attribute.Tag == TagAttribute.TagType.Single)
                    throw new InterpreterException("Tag " + name + " is only single");
                // Create object
                tag = Activator.CreateInstance(tagInfo.Type) as BaseTag;
                if (tag  == null)
                    throw new InterpreterException("Could not convert " + tagInfo.Type + " to BaseTag");

                int i = start + 1;
                if (end > i + 1 && tokens[i].Is("=") && tokens[i + 1].IsDeclaration)
                {
                    //Default
                    AssignDefaultField(tag, tagInfo, tokens[i + 1].Value);
                    i += 2;
                }

                for (; i < end; i += 3) // Attributes
                {
                    if (!tokens[i].IsDeclaration || !tokens[i + 1].Is("=") || !tokens[i + 2].IsDeclaration)
                        throw new InterpreterException("Wrong syntax with attributes at "+name);
                    AssignField(tag, tagInfo, tokens[i].Value, tokens[i + 2].Value);
                }

            }
            else // Generic
            {
                TagReference tagReference = new TagReference();


                int i = start + 1;
                if (end > i + 1 && tokens[i].Is("=") && tokens[i + 1].IsDeclaration)
                {
                    //Default
                    tagReference.Attributes["value"] = tokens[i + 1].Value;
                    i += 2;
                }

                for (; i < end; i += 3) // Attributes
                {
                    if (!tokens[i].IsDeclaration || !tokens[i + 1].Is("=") || !tokens[i + 2].IsDeclaration)
                        throw new InterpreterException("Wrong syntax at attribute declaration of "+name);

                    tagReference.Attributes[tokens[i].Value] = tokens[i + 2].Value;
                }

                
                tag = tagReference;
            }

            tag.Name = name;
            tag.IsSingle = isSingle;
            return tag;

        }

        private static void AssignField(object tagObject, TagTypeInfo typeInfo, string field, string value)
        {
            if (typeInfo.Fields.TryGetValue(field, out TagFieldInfo tagFieldInfo))
            {
                SetFieldValue(tagObject, tagFieldInfo, value);
            }
            else
                throw new InterpreterException("Field: " + field + " of " + typeInfo.Type + " could not be found");
        }

        private static void AssignDefaultField(object tagObject, TagTypeInfo typeInfo, string value)
        {
            foreach (var tagFieldInfo in typeInfo.Fields.Values)
            {
                if (tagFieldInfo.Attribute.IsDefault)
                {
                    SetFieldValue(tagObject, tagFieldInfo, value);
                    return;
                }
            }
            throw new InterpreterException("Default field of " + typeInfo.Type + "was not found");
        }

        private static void SetFieldValue(object tagObject, TagFieldInfo tagFieldInfo, string value)
        {
            try
            {
                object v = Convert.ChangeType(value, tagFieldInfo.FieldInfo.FieldType,CultureInfo.InvariantCulture);
                tagFieldInfo.FieldInfo.SetValue(tagObject, v);
            }
            catch (Exception)
            {
                throw new InterpreterException("Can not assign value: " + value + " to field: " + tagFieldInfo.FieldInfo.Name);
            }
        }


        #endregion

        
    }

    
}
