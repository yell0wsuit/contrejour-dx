using System;

using Mokus2D.Util;
using Mokus2D.Visual;

namespace Mokus2D.UI.Containers;

public class ViewSwitcher : Node
{
    public Action<Node> ShowEffect;

    public Action<Node, Action> HideEffect;

    private Node _currentView;

    private Node _previousView;

    private readonly Action _showCurrentView;

    private readonly Action _onPreviousViewHide;

    public bool RemoveViewManualy;

    public Node CurrentView
    {
        get => _currentView;
        set
        {
            if (_currentView != value)
            {
                _previousView = _currentView;
                _currentView = value;
                if (_previousView != null)
                {
                    HidePreviousAndShowCurrentView();
                }
                else
                {
                    ShowCurrentView();
                }
            }
        }
    }

    public event Action<Node> BeforeShowEvent;

    public event Action<Node> AfterHideEvent;

    public ViewSwitcher()
    {
        _showCurrentView = ShowCurrentView;
        _onPreviousViewHide = OnPreviousViewHide;
    }

    public virtual void ForceShowView(Node view)
    {
        if (_currentView != view)
        {
            _currentView?.RemoveFromParent();
            _currentView = view;
            view.InteractionsEnabled = true;
            view.Visible = true;
            view.OpacityFloat = 1f;
            AddChild(view);
        }
    }

    private void ShowCurrentView()
    {
        if (_currentView != null)
        {
            BeforeShowEvent.Dispatch(_currentView);
            _currentView.InteractionsEnabled = true;
            AddChild(_currentView);
            if (_currentView is IViewStackPage)
            {
                ((IViewStackPage)_currentView).OnShow();
            }
            if (_currentView is IShow)
            {
                ((IShow)_currentView).Show();
            }
            else if (ShowEffect is not null and not null)
            {
                ShowEffect(_currentView);
            }
        }
    }

    private void HidePreviousAndShowCurrentView()
    {
        _previousView.InteractionsEnabled = false;
        if (_previousView is IViewStackPage)
        {
            ((IViewStackPage)_previousView).OnHide();
        }
        if (_previousView is IHide)
        {
            ((IHide)_previousView).Hide(_onPreviousViewHide);
            return;
        }
        if (HideEffect != null)
        {
            HideEffect(_previousView, _onPreviousViewHide);
            return;
        }
        RemovePreviousView();
        ShowCurrentView();
    }

    private void OnPreviousViewHide()
    {
        RemovePreviousView();
        ShowCurrentView();
    }

    private void RemovePreviousView()
    {
        if (!RemoveViewManualy)
        {
            _previousView.RemoveFromParent();
        }
        AfterHideEvent.Dispatch(_previousView);
        _previousView = null;
    }
}
