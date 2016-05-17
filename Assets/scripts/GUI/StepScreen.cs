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
// - Fichier créé le 4/20/2015 11:48:59 AM
// - Dernière modification le 4/20/2015 11:48:59 AM
//@END-HEADER
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace dassault
{
    /// <summary>
    /// description for class StepScreen
    /// </summary>
    public class StepScreen : MonoBehaviour
    {
        void Awake ()
		{
			Init ();
		}

		public void Init()
        {
			if(m_stepViews == null)
			{
				m_stepViews = new List<StepView>();
				m_currentView = null;
				m_isVisible = false;
				ResetOriginalPosition();
				Show (false);
			}
        }

		public void Show(bool show)
		{
			Init();
			if(show != m_isVisible)
			{
				if(show)
				{
					(transform as RectTransform).position = m_originalPosition;
				}
				else
				{
					(transform as RectTransform).position = m_originalPosition + new Vector3(2000, 0, 0);
				}
				m_isVisible = show;
			}
		}

		public void ResetOriginalPosition()
		{
			m_originalPosition = (transform as RectTransform).position;
		}
		
		public bool IsShown()
		{
			return m_isVisible;
		}
		
		public StepView CreateStepView()
		{
			StepView result = m_stepViewContainer.CreateStepView();
			m_stepViews.Add(result);
			return result;
		}

		public void PopStepView()
		{
			if(m_stepViews.Count > 0)
			{
				int lastIndex = m_stepViews.Count-1;
				StepView toRemove = m_stepViews[lastIndex];
				m_stepViewContainer.RemoveStepView(toRemove);
				m_stepViews.RemoveAt(lastIndex);
			}
		}

		public void RemoveLastSteps(int stepToRemoveCount)
		{
			for(int i = 0; i < stepToRemoveCount; ++i)
			{
				PopStepView();
			}
		}

		private void OnPreviousViewHidden(bool shown)
		{
			if(m_currentView != null)
				m_currentView.Show(true);
		}

		private SimpleView.OnTransitionFinishedEvent SelectCallbackBasedOnView(SimpleView view)
		{
			if(view == m_perspectiveView)
			{
				return OnLocalizationTransitionFinished;
			}
			else if(view == m_commentView)
			{
				return OnCommentViewTransitionFinished;
			}
			else if(view == m_referenceView)
			{
				return OnReferenceTransitionFinished;
			}
			else if(view == m_annotationView)
			{
				return OnAnnotationTransitionFinished;
			}
			else if(view == m_toolsView)
			{
				return OnToolsTransitionFinished;
			}
			else
			{
				return null;
			}
		}

		private void AddTransitionCallbackBasedOnView(SimpleView view)
		{
			SimpleView.OnTransitionFinishedEvent callback = SelectCallbackBasedOnView(view);
			if(callback != null)
			{
				view.OnTransitionFinishedCallback += callback;
			}
		}
		
		private void SetCurrentView(SimpleView newView, bool show, SimpleView.OnTransitionFinishedEvent onViewTransitionFinished)
		{
			if(m_currentView != newView && m_currentView != null)
			{
				if(show)
				{
					SimpleView previousView = m_currentView;
					m_currentView = newView;
					previousView.OnTransitionFinishedCallback += OnPreviousViewHidden;
					AddTransitionCallbackBasedOnView(previousView);

					if(onViewTransitionFinished != null)
						newView.OnTransitionFinishedCallback += onViewTransitionFinished;
					AddTransitionCallbackBasedOnView(newView);
					previousView.Show(false);
				}
			}
			else
			{
				if(show)
				{
					m_currentView = newView;
					if(onViewTransitionFinished != null)
						m_currentView.OnTransitionFinishedCallback += onViewTransitionFinished;
					AddTransitionCallbackBasedOnView(m_currentView);
					m_currentView.Show(true);
				}
				else
				{
					if(onViewTransitionFinished != null)
						newView.OnTransitionFinishedCallback += onViewTransitionFinished;
					AddTransitionCallbackBasedOnView(m_currentView);
					newView.Show(false);
					m_currentView = null;
				}
			}
		}

		public void CloseCurrentView()
		{
			if(m_currentView != null)
			{
				AddTransitionCallbackBasedOnView(m_currentView);
				m_currentView.Show(false);
				m_currentView = null;
			}
		}

		private bool AnyViewInTransition()
		{
			return 	m_perspectiveView.IsInTransition() ||
					m_commentView.IsInTransition() ||
					m_referenceView.IsInTransition() ||
					m_annotationView.IsInTransition() ||
					m_toolsView.IsInTransition();
        }
        
		public void ShowCommentView(bool show, SimpleView.OnTransitionFinishedEvent onViewTransitionFinished)
		{
			if(!AnyViewInTransition())
				SetCurrentView(m_commentView, show, onViewTransitionFinished);
		}

		public bool IsCommentViewShown()
		{
			return m_commentView.IsShown ();
		}

		public void ShowReferenceView(bool show, SimpleView.OnTransitionFinishedEvent onViewTransitionFinished)
		{
			if(!AnyViewInTransition())
				SetCurrentView(m_referenceView, show, onViewTransitionFinished);
		}

		public void ShowImageInReferenceView(bool show)
		{
			m_referenceView.ShowImage(show);
		}
		
		public bool IsReferenceViewShown()
		{
			return m_referenceView.IsShown();
		}

		public void ShowLocalizationView(bool show, SimpleView.OnTransitionFinishedEvent onViewTransitionFinished)
		{
			if(!AnyViewInTransition())
				SetCurrentView(m_perspectiveView, show, onViewTransitionFinished);
		}
		
		public bool IsLocalizationViewShown()
		{
            return m_perspectiveView.IsShown();
        }

		public void ShowAnnotationView(bool show, SimpleView.OnTransitionFinishedEvent onViewTransitionFinished)
		{
			if(!AnyViewInTransition())
				SetCurrentView(m_annotationView, show, onViewTransitionFinished);
		}
		
		public void ShowImageInAnnotationView(bool show)
		{
			m_annotationView.ShowImage(show);
		}

		public bool IsAnnotationViewShown()
		{
            return m_annotationView.IsShown();
        }
        
		public void ShowToolsView(bool show, SimpleView.OnTransitionFinishedEvent onViewTransitionFinished)
		{
			if(!AnyViewInTransition())
				SetCurrentView(m_toolsView, show, onViewTransitionFinished);
		}
		
		public bool IsToolsViewShown()
		{
			return m_toolsView.IsShown();
        }
        
		public void ShowAnnotationReceptionView(bool show)
		{
			m_annotationReceptionView.gameObject.SetActive(show);
			m_annotationReceptionView.Show(show);
		}
		
		public bool IsAnnotationReceptionShown()
		{
			return m_toolsView.IsShown();
        }
        
        public void SetCommentValue(string comment, string type, int currentStep, int stepCount)
		{
			m_commentView.SetText(comment, type);
			m_commentView.SetProgress(currentStep, stepCount);
		}

		public void ShowButtons(bool showCommentButton, string commentType, bool showReferenceButton, bool showToolButton, bool showAnnotationButton)
		{
			m_commentButtonHandler.Show(showCommentButton);
			m_commentButtonHandler.SetCommentType(commentType);
			m_referenceButton.SetActive(showReferenceButton);
			m_toolButton.SetActive(showToolButton);
			m_annotationButton.SetActive(showAnnotationButton);
        }
        
		public void SetImage(Texture image, string title, int currentStep, int stepCount)
		{
			m_referenceView.SetImage(image);
			m_referenceView.ShowImage(true);
			m_referenceView.SetTitle(title);
			m_referenceView.SetProgress(currentStep, stepCount);
		}

		public void ShowReferenceAnimation(string title, int currentStep, int stepCount)
		{
			m_referenceView.SetTitle(title);
			m_referenceView.ShowImage(false);
			m_referenceView.SetProgress(currentStep, stepCount);
        }
        
		public void SetAnnotation(Texture image, string title, int currentStep, int stepCount)
		{
			m_annotationView.SetImage(image);
			m_annotationView.SetTitle(title);
			m_annotationView.SetProgress(currentStep, stepCount);
        }
        
		public void SetToolImage(Texture image, string title, int currentStep, int stepCount)
		{
			m_toolsView.SetImage(image);
			m_toolsView.ShowImage(true);
			m_toolsView.SetTitle(title);
			m_toolsView.SetProgress(currentStep, stepCount);
        }

		public void SetWatchConnexion(bool connected)
		{
			if(connected)
				m_glassConnexionImage.color = Color.green;
			else
				m_glassConnexionImage.color = Color.red;
		}
		
		public void SetPadConnexion(bool connected)
		{
			if(connected)
				m_padConnexionImage.color = Color.green;
			else
				m_padConnexionImage.color = Color.red;
		}

		public void ShowStatusBar(bool show)
		{
			m_statusBar.SetActive(show);
		}
		
		public Rect GetPlaneUpViewViewport()
		{
			return GUIHelper.GetRectInViewportSpace(m_planeUpViewViewport, m_rootCanvas);
		}

		public Rect GetPerspectiveViewport()
		{
			return GUIHelper.GetRectInViewportSpace(m_perspectiveView.Content, m_rootCanvas);
		}

		public Rect GetAnimationViewport()
		{
			return GUIHelper.GetRectInViewportSpace(m_referenceView.Content, m_rootCanvas);
        }

		public void OnCommentButtonClicked()
		{
			if(OnSwitchCommentButtonEvent != null)
			{
				OnSwitchCommentButtonEvent();
			}
		}
		
		public void OnImageButtonClicked()
		{
			if(OnSwitchImageButtonEvent != null)
			{
				OnSwitchImageButtonEvent();
			}
		}

		public void OnToolsButtonClicked()
		{
			if(OnSwitchToolsButtonEvent != null)
			{
				OnSwitchToolsButtonEvent();
			}
		}
		
		public void OnAnnotationButtonClicked()
		{
			if(OnSwitchAnnotationButtonEvent != null)
			{
				OnSwitchAnnotationButtonEvent();
			}
		}
		
		public void OnPreviousButtonClicked()
		{
			if(OnPrevButtonEvent != null)
			{
				OnPrevButtonEvent();
			}
		}
		
		public void OnNextButtonClicked()
		{
			if(OnNextButtonEvent != null)
			{
				OnNextButtonEvent();
			}
		}
		
		public void OnLocalizationButtonClicked()
		{
			if(OnSwitchLocalizationEvent != null)
			{
				OnSwitchLocalizationEvent();
			}
		}

		private List<StepView> m_stepViews;
		private SimpleView m_currentView;
		private Vector3 m_originalPosition;
		private bool m_isVisible;
		[SerializeField] private Canvas m_rootCanvas;
        [SerializeField] private StepViewContainer m_stepViewContainer;
		[SerializeField] private RectTransform m_planeUpViewViewport;
		[SerializeField] private SimpleView m_perspectiveView;
		[SerializeField] private TextView m_commentView;
		[SerializeField] private ImageView m_referenceView;
		[SerializeField] private ImageView m_annotationView;
		[SerializeField] private ImageView m_toolsView;
		[SerializeField] private SimpleView m_annotationReceptionView;

        
        [SerializeField] private GameObject m_annotationButton;
		[SerializeField] private GameObject m_toolButton;
		[SerializeField] private GameObject m_referenceButton;
		[SerializeField] private CommentButtonHandler m_commentButtonHandler;

		[SerializeField] private Image m_glassConnexionImage;
		[SerializeField] private Image m_padConnexionImage;

		[SerializeField] private GameObject m_statusBar;

		public delegate void OnButtonClick();
		public event OnButtonClick OnPrevButtonEvent;
		public event OnButtonClick OnNextButtonEvent;
        public event OnButtonClick OnSwitchCommentButtonEvent;
		public event OnButtonClick OnSwitchImageButtonEvent;
		public event OnButtonClick OnSwitchToolsButtonEvent;
		public event OnButtonClick OnSwitchAnnotationButtonEvent;
        public event OnButtonClick OnSwitchLocalizationEvent;

		public SimpleView.OnTransitionFinishedEvent OnCommentViewTransitionFinished;
		public SimpleView.OnTransitionFinishedEvent OnReferenceTransitionFinished;
		public SimpleView.OnTransitionFinishedEvent OnToolsTransitionFinished;
		public SimpleView.OnTransitionFinishedEvent OnAnnotationTransitionFinished;
		public SimpleView.OnTransitionFinishedEvent OnLocalizationTransitionFinished;

	}
}
