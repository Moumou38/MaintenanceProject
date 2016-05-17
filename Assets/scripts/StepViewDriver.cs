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
// - Fichier créé le 4/23/2015 10:35:07 AM
// - Dernière modification le 4/23/2015 10:35:07 AM
//@END-HEADER
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace dassault
{
    /// <summary>
    /// description for class StepDriver
    /// </summary>
    public class StepViewDriver
    {

		public StepViewDriver(PlaneViewModule planeViewModule, AnimationModule animationModule, StepScreen stepScreen, bool registerGUIEvents)
		{
			m_stepScreen = stepScreen;
			m_planeViewModule = planeViewModule;
			m_animationModule = animationModule;
#if UNITY_ANDROID
			m_scenarioFolder = "/mnt/sdcard/DispositifMainLibre/Scenarii/";
#else
			m_scenarioFolder = Application.dataPath + "/../Datas/scenarii/";
#endif
			m_previousHierachy = new List<Step>();

			m_registerGUIEvents = registerGUIEvents;
			if(m_registerGUIEvents)
			{
				m_stepScreen.OnSwitchCommentButtonEvent += SwitchCommentView;
				m_stepScreen.OnSwitchImageButtonEvent += SwitchImageView;
				m_stepScreen.OnSwitchLocalizationEvent += SwitchLocalizationView;
				m_stepScreen.OnSwitchToolsButtonEvent += SwitchToolsView;
				m_stepScreen.OnNextButtonEvent += ShowCurrentViewNextInfo;
				m_stepScreen.OnPrevButtonEvent += ShowCurrentViewPrevInfo;
				m_stepScreen.OnSwitchAnnotationButtonEvent += SwitchAnnotationView;
			}

			m_stepScreen.OnCommentViewTransitionFinished = OnCommentViewVisibilityChanged;
			m_stepScreen.OnToolsTransitionFinished = OnToolViewVisibilityChanged;
			m_stepScreen.OnReferenceTransitionFinished = OnReferenceViewVisibilityChanged;
			m_stepScreen.OnAnnotationTransitionFinished = OnAnnotationViewVisibilityChanged;
			m_stepScreen.OnLocalizationTransitionFinished = OnLocalizationViewVisibilityChanged;
            
            m_planeViewModule.SetOrthoCameraViewport(m_stepScreen.GetPlaneUpViewViewport());
			m_planeViewModule.ShowOrthoView(true);
			
			m_planeViewModule.SetPerspectiveCameraViewport(m_stepScreen.GetPerspectiveViewport());
			m_planeViewModule.ShowPerspectiveView(false);

			m_animationModule.SetCameraViewport(m_stepScreen.GetAnimationViewport());
			m_animationModule.Activate(false);

			m_stepScreen.Init();

			
		}

        public void initObjects(TextAsset iScenario, Dictionary<string, Object> iObjectList)
        {
            m_figures = iObjectList;
            LoadScenario(iScenario); 
        }

		public void UpdateViewports()
		{
			m_planeViewModule.SetOrthoCameraViewport(m_stepScreen.GetPlaneUpViewViewport());
			m_planeViewModule.SetPerspectiveCameraViewport(m_stepScreen.GetPerspectiveViewport());
			m_animationModule.SetCameraViewport(m_stepScreen.GetAnimationViewport());
		}

        public void LoadScenario(TextAsset iScenario)
		{
            m_scenarioName = iScenario;
			m_stepRoot = new Step();
			m_stepRoot.ReadFromFile(iScenario);
			SetCurrentStep(m_stepRoot.GetFirstLeaf());
			m_bookmarks = new List<Step>();
			m_stepRoot.GetBookmarkedSubStep(m_bookmarks);
		}

		public void Show(bool show)
		{
			m_stepScreen.Show(show);
			m_planeViewModule.Activate(show);
		}

		public ScenarioState GetCurrentState()
		{
			ScenarioState result = new ScenarioState(GetCurrentStepPath(),
		                                         	 IsCommentsShown(), 
			                                         IsReferencesShown(), 
			                                         IsToolsShown(),
			                                         IsAnnotationsShown(),
			                                         m_currentCommentIndex,
			                                         m_currentReferenceIndex,
			                                         m_currentToolIndex,
			                                         m_currentAnnotationIndex,
			                                         IsLocalizationShown(),
			                               			 IsGuiShown());
			return result;
		}

		public void SetCurrentState(ScenarioState state)
		{
			Show(true);
			m_planeViewModule.ShowOrthoView(true);

			bool isCommentViewShown = m_stepScreen.IsCommentViewShown();
			bool isLocalizationViewShown = m_stepScreen.IsLocalizationViewShown();
			bool isReferenceViewShown = m_stepScreen.IsReferenceViewShown();
			bool isToolViewShown = m_stepScreen.IsToolsViewShown();
			bool isAnnotationViewShown = m_stepScreen.IsAnnotationViewShown();
            
            SetCurrentStepByPath(state.StepPath);
			m_currentCommentIndex = state.CurrentCommentIndex;
			m_currentReferenceIndex = state.CurrentReferenceIndex;
			m_currentToolIndex = state.CurrentToolIndex;
			m_currentAnnotationIndex = state.CurrentAnnotationIndex;
			if(state.IsCommentsVisible)
			{
				m_stepScreen.ShowCommentView(true, SetCurrentCommentIndexAfterOpening);
				if(isCommentViewShown)
					SetCurrentCommentIndexAfterOpening(true);
			}
			else if(state.IsLocalizationVisible)
			{
				m_stepScreen.ShowLocalizationView(true, OnPerspectiveViewTransitionFinished);
				if(isLocalizationViewShown)
                    OnPerspectiveViewTransitionFinished(true);
			}
			else if(state.IsReferencesVisible)
			{
				m_stepScreen.ShowReferenceView(true, SetReferenceIndexAfterOpening);
				if(isReferenceViewShown)
					SetReferenceIndexAfterOpening(true);
			}
			else if(state.IsToolsVisible)
			{
				m_stepScreen.ShowToolsView(true, SetReferenceIndexAfterOpening);
				if(isToolViewShown)
					SetReferenceIndexAfterOpening(true);
			}
			else if(state.IsAnnotationsVisible)
			{
				m_stepScreen.ShowAnnotationView(true, SetCurrentAnnotationIndexAfterOpening);
				if(isAnnotationViewShown)
					SetCurrentAnnotationIndexAfterOpening(true);
			}
			else
			{
				m_stepScreen.CloseCurrentView();
			}
			if(!state.IsGuiVisible)
			{
				SwitchVisibility();
			}
		}

		public bool IsLocalizationShown()
		{
			return m_stepScreen.IsLocalizationViewShown();
		}
		
		public bool IsCommentsShown()
		{
			return m_stepScreen.IsCommentViewShown();
		}
		
		public bool IsReferencesShown()
		{
			return m_stepScreen.IsReferenceViewShown();
		}
		
		public bool IsToolsShown()
		{
			return m_stepScreen.IsToolsViewShown();
		}
		
		public bool IsAnnotationsShown()
		{
			return m_stepScreen.IsAnnotationViewShown();
		}

		public bool IsGuiShown()
		{
			return m_stepScreen.IsShown();
		}

		public bool HasComments()
		{
            if (m_currentStep != null)
                return m_currentStep.Comments.Count >= 1;
            else
                return false;
		}
		
		public bool HasReferences()
		{
            if (m_currentStep != null)
                return m_currentStep.References.Count >= 1;
            else
                return false; 
		}
		
		public bool HasTools()
		{
            if (m_currentStep != null)
                return m_currentStep.Tools.Count >= 1;
            else
                return false; 
		}
		
		public bool HasAnnotations()
		{
            if (m_currentStep != null)
			    return m_currentStep.Annotations.Count >= 1;
            else
            return false; 
		}

		public bool HasNextComment()
		{
			return m_currentCommentIndex < m_currentStep.Comments.Count - 1;
		}
		
		public bool HasNextReference()
		{
			return m_currentReferenceIndex < m_currentStep.References.Count - 1;
		}
		
		public bool HasNextTool()
		{
			return m_currentToolIndex < m_currentStep.Tools.Count - 1;
		}
		
		public bool HasNextAnnotation()
		{
			return m_currentAnnotationIndex < m_currentStep.Annotations.Count - 1;
		}
		
		public void ShowCurrentViewNextInfo()
		{
			if(m_stepScreen.IsCommentViewShown())
			{
				if(m_currentCommentIndex < m_currentStep.Comments.Count - 1)
				{
					SetCurrentCommentIndex(m_currentCommentIndex+1);
                }
			}
			else if(m_stepScreen.IsReferenceViewShown())
			{
				if(m_currentReferenceIndex < m_currentStep.References.Count - 1)
				{
					SetCurrentReferenceIndex(m_currentReferenceIndex+1);
                }
            }
			else if(m_stepScreen.IsAnnotationViewShown())
			{
				if(m_currentAnnotationIndex < m_currentStep.Annotations.Count - 1)
				{
					SetCurrentAnnotationIndex(m_currentAnnotationIndex+1);
                }
            }
			else if(m_stepScreen.IsToolsViewShown())
			{
				if(m_currentToolIndex < m_currentStep.Tools.Count - 1)
				{
					SetCurrentToolIndex(m_currentToolIndex+1);
                }
            }
            
		}
		
		public void ShowCurrentViewPrevInfo()
		{
			if(m_stepScreen.IsCommentViewShown())
			{
				if(m_currentCommentIndex > 0)
				{
					SetCurrentCommentIndex(m_currentCommentIndex-1);
				}
			}
			else if(m_stepScreen.IsReferenceViewShown())
			{
				if(m_currentReferenceIndex > 0)
				{
					SetCurrentReferenceIndex(m_currentReferenceIndex-1);
				}
			}
			else if(m_stepScreen.IsAnnotationViewShown())
			{
				if(m_currentAnnotationIndex > 0)
				{
					SetCurrentAnnotationIndex(m_currentAnnotationIndex-1);
				}
			}
			else if(m_stepScreen.IsToolsViewShown())
			{
				if(m_currentToolIndex > 0)
				{
					SetCurrentToolIndex(m_currentToolIndex-1);
				}
			}
		}

		private void OnCommentViewVisibilityChanged(bool visible)
		{
			if(OnCommentViewVisibilityChangedCallback != null)
				OnCommentViewVisibilityChangedCallback(visible);
        }
        
		private void OnToolViewVisibilityChanged(bool visible)
		{
			if(OnToolViewVisibilityChangedCallback != null)
				OnToolViewVisibilityChangedCallback(visible);
        }
        
		private void OnReferenceViewVisibilityChanged(bool visible)
		{
			if(OnReferenceViewVisibilityChangedCallback != null)
				OnReferenceViewVisibilityChangedCallback(visible);
        }
        
		private void OnAnnotationViewVisibilityChanged(bool visible)
		{
			if(OnAnnotationViewVisibilityChangedCallback != null)
				OnAnnotationViewVisibilityChangedCallback(visible);
        }
        
		private void OnLocalizationViewVisibilityChanged(bool visible)
		{
			if(OnLocalizationViewVisibilityChangedCallback != null)
				OnLocalizationViewVisibilityChangedCallback(visible);
        }
        
        public void SwitchCommentView()
		{
			bool show = !m_stepScreen.IsCommentViewShown() && HasComments();
			m_stepScreen.ShowCommentView(show, SetCurrentCommentIndexAfterOpening);
			if(show)
			{
				m_planeViewModule.ShowPerspectiveView(false);
				m_animationModule.Activate(false);
			}
        }

		private void SetCurrentCommentIndexAfterOpening(bool show)
		{
			if(show)
			{
				SetCurrentCommentIndex(m_currentCommentIndex);
			}
		}
        
        public void SwitchImageView()
		{
			bool show = !m_stepScreen.IsReferenceViewShown() && HasReferences();
			m_stepScreen.ShowReferenceView(show, SetReferenceIndexAfterOpening);
			if(show)
			{
				m_planeViewModule.ShowPerspectiveView(false);
				m_animationModule.Activate(false);
			}
        }

		private void SetReferenceIndexAfterOpening(bool show)
		{
			if(show)
			{
				m_stepScreen.ShowImageInReferenceView(true);
				SetCurrentReferenceIndex(m_currentReferenceIndex);
			}
			else
			{
				m_animationModule.Activate(false);
			}
		}

		public void SwitchLocalizationView()
		{
			bool show = !m_stepScreen.IsLocalizationViewShown();
			m_stepScreen.ShowLocalizationView(show, OnPerspectiveViewTransitionFinished);
			m_animationModule.Activate(false);
        }

		public void SetWatchConnexionStatus(bool connected)
		{
			m_stepScreen.SetWatchConnexion(connected);
		}
		
		public void SetPadConnexionStatus(bool connected)
		{
			m_stepScreen.SetPadConnexion(connected);
		}
		
		public void SwitchVisibility()
		{
			bool show = !m_stepScreen.IsShown();
			if(!show) // on doit sauvegarder l'etat de visibilité des vues avant de cacher l'ecran
			{
				m_animationModule.Activate(false);
				m_planeViewModule.ShowPerspectiveView(false);
			}

			m_stepScreen.Show(show);
			m_planeViewModule.ShowOrthoView(show);

			if(show) // On doit restaurer la visibilité des vues après avoir affiché l'ecran
			{
				if(m_stepScreen.IsCommentViewShown())
				{
					SetCurrentCommentIndexAfterOpening(true);
				}
				else if(m_stepScreen.IsLocalizationViewShown())
				{
					OnPerspectiveViewTransitionFinished(true);
				}
				else if(m_stepScreen.IsReferenceViewShown())
				{
					SetReferenceIndexAfterOpening(true);
				}
				else if(m_stepScreen.IsToolsViewShown())
				{
					SetReferenceIndexAfterOpening(true);
				}
				else if(m_stepScreen.IsAnnotationViewShown())
				{
					SetCurrentAnnotationIndexAfterOpening(true);
				}
			}
		}

		private void OnPerspectiveViewTransitionFinished(bool show)
		{
			m_planeViewModule.ShowPerspectiveView(show);
			if(show)
			{
				m_planeViewModule.StartPerspectiveCameraAnimationToViewHighlightedObject();
			}
		}

		public void SwitchAnnotationView()
		{
			bool show = !m_stepScreen.IsAnnotationViewShown() && HasAnnotations();
			m_stepScreen.ShowAnnotationView(show, SetCurrentAnnotationIndexAfterOpening);
			if(show)
			{
				m_planeViewModule.ShowPerspectiveView(false);
				m_animationModule.Activate(false);
			}
        }

		private void SetCurrentAnnotationIndexAfterOpening(bool show)
		{
			if(show)
			{
				m_stepScreen.ShowImageInAnnotationView(true);
				SetCurrentAnnotationIndex(m_currentAnnotationIndex);
			}
		}

		public void SwitchToolsView()
		{
			bool show = !m_stepScreen.IsToolsViewShown() && HasTools();
			m_stepScreen.ShowToolsView(show, SetReferenceIndexAfterOpening);
			if(show)
			{
				m_planeViewModule.ShowPerspectiveView(false);
			}
		}
		
		private void SetToolIndexAfterOpening(bool show)
		{
			if(show)
			{
                SetCurrentToolIndex(m_currentToolIndex);
            }
        }
        
		public void NextStep()
		{
            Debug.Log("StepViewDriver.NextStep()");
			Step nextStep = m_currentStep.GetNextLeaf();
            if (nextStep != null)
            {
                SetCurrentStep(nextStep);
            }
            else
            {
                Debug.Log("nextStep is null");
            }
		}
		
		public void PreviousStep()
        {
            Debug.Log("StepViewDriver.PreviousStep()");
			Step previousStep = m_currentStep.GetPreviousLeaf();
            if (previousStep != null)
            {
                SetCurrentStep(previousStep);
            }
            else
            {
                Debug.Log("previousStep is null");
            }
		}

		// public for debug purpose, should be private
		public void SetCurrentStepByPath(string path)
        {
            Debug.Log("StepViewDriver.SetCurrentStepByPath()");
			SetCurrentStep(m_stepRoot.GetStepByPath(path));
		}

		public void AddAnnotationToStep(Texture annotationImage, string stepPath)
		{
			Step step = m_stepRoot.GetStepByPath(stepPath);
			if(step != null)
			{
				m_receivedAnnotationStepPath = stepPath;
				step.Annotations.Add(annotationImage);
				m_stepScreen.ShowAnnotationReceptionView(true);
				string commentType = m_currentStep.GetCommentMostImportantType();
				m_stepScreen.ShowButtons(HasComments(), commentType,
				                         HasReferences(),
				                         HasTools(),
				                         HasAnnotations());
			}
			else
			{
				m_receivedAnnotationStepPath = null;
            }
		}

		public void OpenReceivedAnnotationStep()
		{
			if(!string.IsNullOrEmpty(m_receivedAnnotationStepPath))
			{
				SetCurrentStepByPath(m_receivedAnnotationStepPath);
				m_currentAnnotationIndex = m_currentStep.Annotations.Count - 1;
				m_stepScreen.ShowAnnotationView(true, SetCurrentAnnotationIndexAfterOpening);
				m_planeViewModule.ShowPerspectiveView(false);
				m_animationModule.Activate(false);
            }
			HideAnnotationReceptionView();
		}

		public void HideAnnotationReceptionView()
		{
			m_stepScreen.ShowAnnotationReceptionView(false);
        }

		public string GetCurrentStepPath()
		{
			return m_currentStep.GetPath();
		}

		public void SetCurrentStepAsBookmark(int bookmarkIndex)
		{
			if(bookmarkIndex >= 0 && bookmarkIndex < m_bookmarks.Count)
			{
				SetCurrentStep(m_bookmarks[bookmarkIndex]);
			}
		}

		private void SetCurrentStep(Step currentStep)
		{
			m_currentStep = currentStep;
			Debug.Log (m_currentStep.GetPath());
			SetCurrentCommentIndex(0);
			SetCurrentReferenceIndex(0);
			SetCurrentAnnotationIndex(0);
			SetCurrentToolIndex(0);
			string commentType = m_currentStep.GetCommentMostImportantType();
			m_stepScreen.ShowButtons(HasComments(), commentType,
			                         HasReferences(),
			                         HasTools(),
			                         HasAnnotations());
			HideAnnotationReceptionView();
			List<Step> hierachy = m_currentStep.GetHierarchy();
			int numberOfStepToRemove = GetNumberOfStepsToRemove(m_previousHierachy, hierachy);
			m_stepScreen.RemoveLastSteps(numberOfStepToRemove);
			for(int i = 1; i < hierachy.Count; ++i) // On supprime le premier element de la liste
			{
				Step step = hierachy[i];
				if(i < m_previousHierachy.Count && m_previousHierachy[i] == step)
					continue;
				StepView stepView = m_stepScreen.CreateStepView();
				string overview = "";
				if(i == hierachy.Count - 1)
				{
					stepView.SetContent(step.Instruction);
					if(m_registerGUIEvents)
					{
						stepView.OnLeftButtonClickCallback += PreviousStep;
						stepView.OnRightButtonClickCallback += NextStep;
					}
				}
				else
				{
					string title = step.Title;
					if(title.Length < 70)
					{
						overview = title;
					}
					else
					{
						overview = title.Substring(0, 70) + "[...]";
					}
				}
				int index = step.GetIndexInParent();
				int count = step.GetSiblingCount();
				stepView.SetOverview(index, count, overview);
			}
			m_previousHierachy = hierachy;
			m_planeViewModule.ShowPerspectiveView(false);
			m_animationModule.Activate(false);
			m_planeViewModule.SetHighlightPart(m_currentStep.Target);//"Trappe_Mecanicien_Gauche"
			m_stepScreen.CloseCurrentView();
			if(OnStepChangedCallback != null)
				OnStepChangedCallback(m_currentStep.GetPath());
		}

		private int GetNumberOfStepsToRemove(List<Step> previousHierachy, List<Step> currentHierarchy)
		{
			int i = 0;
			while(i < previousHierachy.Count && i < currentHierarchy.Count && previousHierachy[i] == currentHierarchy[i])
			{
				++i;
			}
			int result = previousHierachy.Count - i;
			return result;
		}
        
        private void SetCurrentCommentIndex(int commentIndex)
		{
			m_currentCommentIndex = commentIndex;
			if(HasComments() && m_currentCommentIndex < m_currentStep.Comments.Count)
			{
				Comment comment = m_currentStep.Comments[m_currentCommentIndex];
				m_stepScreen.SetCommentValue(comment.Content, comment.Type, m_currentCommentIndex, m_currentStep.Comments.Count);
			}
		}
		
		private void SetCurrentReferenceIndex(int referenceIndex)
		{
			m_currentReferenceIndex = referenceIndex;
			if(HasReferences() && m_currentReferenceIndex < m_currentStep.References.Count)
			{
				Reference reference = m_currentStep.References[m_currentReferenceIndex];
				if(reference.Type == "figure")
				{
					m_animationModule.Activate(false);

                    Texture2D tex = m_figures[reference.Link] as Texture2D; 

					m_stepScreen.SetImage(tex, reference.Title, m_currentReferenceIndex, m_currentStep.References.Count);
                }
				else if(reference.Type == "animation")
				{
					m_stepScreen.ShowReferenceAnimation(reference.Title, m_currentReferenceIndex, m_currentStep.References.Count);
					m_animationModule.Activate(true);
					m_animationModule.PlayAnimation(reference.Name, reference.Link);
				}
            }
        }
        
		private void SetCurrentAnnotationIndex(int annotationIndex)
		{
			m_currentAnnotationIndex = annotationIndex;
			if(HasAnnotations() && m_currentAnnotationIndex < m_currentStep.Annotations.Count)
			{
				Texture annotation = m_currentStep.Annotations[m_currentAnnotationIndex];
				m_stepScreen.SetAnnotation(annotation, m_currentStep.GetPath(), m_currentAnnotationIndex, m_currentStep.Annotations.Count);
            }
        }
        
		private void SetCurrentToolIndex(int toolIndex)
		{
			m_currentToolIndex = toolIndex;
			if(HasTools() && m_currentToolIndex < m_currentStep.Tools.Count)
			{
				Tool tool = m_currentStep.Tools[m_currentToolIndex];
				Texture2D tex = new Texture2D(4, 4);
				string fileName = m_scenarioFolder + m_scenarioName + "/" + tool.Image;
				if(File.Exists(fileName))
				{
					byte[] imageContent = File.ReadAllBytes(fileName);
					tex.LoadImage(imageContent); // LoadImage replace the texture content
				}
				m_stepScreen.SetToolImage(tex, tool.Name, m_currentToolIndex, m_currentStep.Tools.Count);
            }
        }
        
        private StepScreen m_stepScreen;
        private PlaneViewModule m_planeViewModule;
		private AnimationModule m_animationModule;
		private Step m_stepRoot;
		private Step m_currentStep;
		private List<Step> m_previousHierachy;
		private List<Step> m_bookmarks;
		private int m_currentCommentIndex;
		private int m_currentReferenceIndex;
		private int m_currentAnnotationIndex;
		private int m_currentToolIndex;
		private string m_scenarioFolder;
		private TextAsset m_scenarioName;
		private string m_receivedAnnotationStepPath;
        private Dictionary<string, Object> m_figures; 

		private bool m_commentViewWasVisible = false;
		private bool m_perspectiveViewWasVisible = false;
		private bool m_imageViewWasVisible = false;
		private bool m_toolsViewWasVisible = false;
		private bool m_anotationViewWasVisible = false;

		private bool m_registerGUIEvents;

		public delegate void OnStepChangedDelegate(string stepPath);
		public event OnStepChangedDelegate OnStepChangedCallback;

		public delegate void OnViewVisibilityChangedDelagate(bool visible);
		public event OnViewVisibilityChangedDelagate OnCommentViewVisibilityChangedCallback;
		public event OnViewVisibilityChangedDelagate OnToolViewVisibilityChangedCallback;
		public event OnViewVisibilityChangedDelagate OnReferenceViewVisibilityChangedCallback;
		public event OnViewVisibilityChangedDelagate OnAnnotationViewVisibilityChangedCallback;
		public event OnViewVisibilityChangedDelagate OnLocalizationViewVisibilityChangedCallback;
        
    }
}
