// Copyright 2015 Dassault
//
// - Description
//     
//
// - Namespace(s)
//    dassault
//
// - Auteurs
//    \author Michel de Verdelhan <mdeverdelhan@theoris.fr>
//
// - Fichier créé le 4/20/2015 6:20:58 PM
// - Dernière modification le 4/20/2015 6:20:58 PM
//@END-HEADER
using UnityEngine;
using System.Collections.Generic;
using System.Xml;

namespace dassault
{
    /// <summary>
    /// description for class Step
    /// </summary>
    public class Step
    {
		public Step()
		{
			m_subSteps = new List<Step>();
			m_comments = new List<Comment>();
			m_references = new List<Reference>();
			m_annotations = new List<Texture>();
			m_tools = new List<Tool>();
			m_parent = null;
			m_bookmark = false;
		}

		public string Instruction {get{return m_instruction;}}
		public string Title
		{
			get
			{
				if(string.IsNullOrEmpty(m_title))
					return m_instruction;
				else
					return m_title;
			}
		}
		public string Target
		{
			get
			{
				if(string.IsNullOrEmpty(m_target))
				{
					if(m_parent != null)
						return m_parent.Target;
					else
						return null;
				}
				else
					return m_target;
			}
		}
		public List<Comment> Comments{get{return m_comments;}}
		public List<Reference> References{get{return m_references;}}

		public List<Texture> Annotations{get{return m_annotations;}}
		public List<Tool> Tools{get{return m_tools;}}

		public bool Bookmark{get{return m_bookmark;}}

		public bool isLeaf()
		{
			return m_subSteps.Count == 0;
		}

		public List<Step> GetHierarchy()
		{
			List<Step> result = new List<Step>();
			Step current = this;
			do
			{
				result.Add(current);
				current = current.m_parent;
			}while (current != null);
			result.Reverse();
			return result;
		}

		public void GetBookmarkedSubStep(List<Step> listToFill)
		{
			Step currentStep = null;
			if(isLeaf())
				currentStep = this;
			else
				currentStep = GetFirstLeaf();
			do
			{
				if(currentStep.m_bookmark)
					listToFill.Add(currentStep);
				currentStep = currentStep.GetNextLeaf();
			} while(currentStep != null);
		}

		public Step GetFirstLeaf()
		{
			if(isLeaf())
				return this;
			else
				return m_subSteps[0].GetFirstLeaf();
		}
		
		public Step GetLastLeaf()
		{
			if(isLeaf())
				return this;
			else
				return m_subSteps[m_subSteps.Count - 1].GetLastLeaf();
		}
		
		public Step GetNextLeaf()
		{
			if(!IsLastSibling())
			{
				int nextIndex = GetIndexInParent()+1;
				return m_parent.m_subSteps[nextIndex].GetFirstLeaf();
			}
			else
			{
				Step son = this;
				Step parent = m_parent;
				while(parent != null && son.IsLastSibling())
				{
					son = parent;
					parent = parent.m_parent;
				}
				if(parent != null)
				{
					int nextIndex = son.GetIndexInParent() + 1;
					return parent.m_subSteps[nextIndex].GetFirstLeaf();
				}
				else
				{
					return null;
				}
			}
		}

		public Step GetPreviousLeaf()
		{
			if(!IsFirstSibling())
			{
				int prevIndex = GetIndexInParent()-1;
				return m_parent.m_subSteps[prevIndex].GetLastLeaf();
			}
			else
			{
				Step son = this;
				Step parent = m_parent;
				while(parent != null && son.IsFirstSibling())
				{
					son = parent;
					parent = parent.m_parent;
				}
				if(parent != null)
				{
					int prevIndex = son.GetIndexInParent() - 1;
					return parent.m_subSteps[prevIndex].GetLastLeaf();
				}
				else
				{
					return null;
				}
			}
		}

		public int GetIndexInParent()
		{
			if(m_parent == null)
				return 0;
			else
			{
				Step[] debug = m_parent.m_subSteps.ToArray();
				return m_parent.m_subSteps.IndexOf(this);
			}
		}

		public int GetSiblingCount()
		{
			if(m_parent == null)
				return 1;
			else
				return m_parent.m_subSteps.Count;
		}

		public string GetCommentMostImportantType()
		{
			string result = "";
			foreach(Comment comment in m_comments)
			{
				if(comment.Type == "warning")
					return "warning";
				else if(comment.Type == "caution")
					result = "caution";
				else if(comment.Type == "note" && result != "caution")
					result = "note";
			}
			return result;
		}

		public string GetPath()
		{
			if(m_parent == null)
			{
				return "1/1";
			}
			string parentPath = m_parent.GetPath();
			int indexStartingAt1 = GetIndexInParent() + 1;
			int count = GetSiblingCount();
			string thisLevelPath = ">" + indexStartingAt1.ToString() + "/" + count;
			return parentPath + thisLevelPath;
		}

		public Step GetStepByPath(string path)
		{
            Debug.Log("/////////// PATH  : " + path); 
			string[] steps = path.Split('>');
			List<int> indices = new List<int>();
			foreach(string step in steps)
			{
                
				string[] indexAsString = step.Split ('/');
				int index = int.Parse(indexAsString[0]) - 1;
				indices.Add(index);
			}
			indices.RemoveAt(0); // we are already in first step
			Step currentStep = this;
			foreach(int i in indices)
			{
				if(i < currentStep.m_subSteps.Count)
				{
					currentStep = currentStep.m_subSteps[i];
				}
				else
				{
					return null;
				}
			}
			return currentStep;
		}

		private bool IsLastSibling()
		{
			return GetIndexInParent() == GetSiblingCount() -1;
		}

		private bool IsFirstSibling()
		{
			return GetIndexInParent() == 0;
		}

		public void ReadFromFile(TextAsset file)
		{
			XmlDocument document = new XmlDocument();
            document.LoadXml(file.text);
			XmlNode root = document.FirstChild;
			ReadFromXml(root as XmlElement);
			List<Comment> comments = new List<Comment>();
			List<Reference> references = new List<Reference>();
			List<Tool> tools = new List<Tool>();
			PropagateDatasToLeaf(comments, references, tools);
		}

		private void ReadFromXml(XmlElement element)
		{
			foreach(XmlNode son in element.ChildNodes)
			{
				if(son.Name == "INSTRUCTIONS")
				{
					m_instruction = son.InnerText;
				}
				else if(son.Name == "TITLE")
				{
					m_title = son.InnerText;
				}
				else if(son.Name == "TARGET")
				{
					m_target = son.InnerText;
				}
				else if(son.Name == "COMMENTS")
				{
					ReadComments(son as XmlElement);
				}
				else if(son.Name == "TOOLS")
				{
					ReadTools(son as XmlElement);
				}
				else if(son.Name == "REFERENCES")
				{
					ReadReferences(son as XmlElement);
				}
				else if(son.Name == "STEPS")
				{
					ReadSubSteps(son as XmlElement);
				}
			}
			m_bookmark = element.HasAttribute("bookmark") && element.GetAttribute("bookmark") == "true";
		}

		private void ReadComments(XmlElement element)
		{
			foreach(XmlNode son in element.ChildNodes)
			{
				if(son.Name == "COMMENT")
				{
					string comment = son.InnerText;
					string type = (son as XmlElement).GetAttribute("type");
					m_comments.Add(new Comment(type, comment));
				}
			}
		}
		
		private void ReadTools(XmlElement element)
		{
			foreach(XmlNode reference in element.ChildNodes)
			{
				if(reference.Name == "TOOL")
				{
					string name = "";
					string image = "";
					foreach(XmlNode son in reference.ChildNodes)
					{
						if(son.Name == "NAME")
						{
							name = son.InnerText;
						}
						else if(son.Name == "IMAGE")
						{
							image = son.InnerText;
						}
					}
					m_tools.Add(new Tool(name, image));
				}
			}
		}

		private void ReadReferences(XmlElement element)
		{
			foreach(XmlNode reference in element.ChildNodes)
			{
				if(reference.Name == "REFERENCE")
				{
					string name = "";
					string title = "";
					string link = "";
					string type = (reference as XmlElement).GetAttribute("type");
					foreach(XmlNode son in reference.ChildNodes)
					{
						if(son.Name == "NAME")
						{
							name = son.InnerText;
                        }
						if(son.Name == "TITLE")
						{
							title = son.InnerText;
                        }
                        else if(son.Name == "LINK")
						{
							link = son.InnerText;
						}
					}
					m_references.Add(new Reference(name, title, type, link));
				}
			}
		}

		private void ReadSubSteps(XmlElement element)
		{
			foreach(XmlNode son in element.ChildNodes)
			{
				if(son.Name == "STEP")
				{
					Step step = new Step();
					step.ReadFromXml(son as XmlElement);
					step.m_parent = this;
					m_subSteps.Add(step);
				}
			}
		}

		private void PropagateDatasToLeaf(List<Comment> comments, List<Reference> references, List<Tool> tools)
		{
			if(!isLeaf())
			{
				List<Comment> commentsForSons = comments;
				if(m_comments.Count > 0)
				{
					commentsForSons = new List<Comment>(comments);
					commentsForSons.AddRange(m_comments);
				}
				List<Reference> referencesForSons = references;
				if(m_references.Count > 0)
				{
					referencesForSons = new List<Reference>(references);
					referencesForSons.AddRange(m_references);
                }
				List<Tool> toolsForSons = tools;
				if(m_tools.Count > 0)
				{
					toolsForSons = new List<Tool>(tools);
					toolsForSons.AddRange(m_tools);
                }
                foreach(Step son in m_subSteps)
				{
					son.PropagateDatasToLeaf(commentsForSons, referencesForSons, toolsForSons);
				}
			}
			else
			{
				m_comments.AddRange(comments);
				m_references.AddRange(references);
				m_tools.AddRange(tools);
			}
        }
        
        private string m_instruction;
		private string m_title;
		private string m_target;
		private bool m_bookmark;
		private List<Step> m_subSteps;
		private List<Comment> m_comments;
		private List<Reference> m_references;
		private List<Texture> m_annotations;
		private List<Tool> m_tools;
		private Step m_parent;
	}
}
