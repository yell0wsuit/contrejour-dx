using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Controls;
using Mokus2D.Events;
using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class PlanetsSpinner : Node, IDisposable
{
    public readonly EventSender<int> SelectEvent = new();

    private readonly List<ChapterItem> chapters = [];

    private float currentIndex;

    private ChapterLocked explodingChapter;

    private readonly GesturePager pager = new();

    public Vector2 AccelerometerOffset = Vector2.Zero;

    public float PlanetsScale = 1f;

    private bool hasExplodingChapter;

    private bool exploding;

    public bool HasExplodingChapter => hasExplodingChapter;

    public float CurrentIndex
    {
        get => currentIndex;
        set
        {
            currentIndex = value;
            pager.SetTargetPosition((int)currentIndex);
            pager.CurrentPosition = currentIndex;
        }
    }

    public bool Enabled
    {
        get => pager.Enabled;
        set => pager.Enabled = value;
    }

    public bool Exploding => exploding;

    public PlanetsSpinner(MainMenu menu)
    {
        CreatePlanets(menu);
        RefreshPosition();
    }

    public override void Update(float time)
    {
        pager.Update(time);
        currentIndex = pager.CurrentPosition;
        RefreshPosition();
    }

    private void CreatePlanets(MainMenu menu)
    {
        List<Type> list = [typeof(Chapter1), typeof(Chapter2), typeof(Chapter3), typeof(Chapter4), typeof(Chapter5)
];
        int totalStars = UserData.Instance.TotalStars;
        for (int i = 0; i < list.Count; i++)
        {
            bool flag = ContreJourConditions.Trial(trialValue: false, i < Constants.NormalChaptersCount && UserData.StarsToUnlock(i) > totalStars);
            ChapterItem chapterItem = (ChapterItem)ReflectUtil.CreateInstance(flag ? typeof(ChapterLocked) : list[i], i, menu);
            if (!flag && i >= UserData.Instance.UnlockedChapters && i < Constants.NormalChaptersCount && UserData.StarsToUnlock(i) > 0)
            {
                CreateExplodingChapter(chapterItem, menu);
                hasExplodingChapter = true;
            }
            AddChild(chapterItem);
            chapters.Add(chapterItem);
            chapterItem.SelectEvent += OnSelect;
        }
    }

    private void OnSelect(int index)
    {
        if (Enabled)
        {
            SelectEvent.SendEvent(index);
            pager.SetTargetPosition((int)Math.Round(pager.CurrentPosition));
        }
    }

    private void CreateExplodingChapter(ChapterItem chapter, MainMenu menu)
    {
        exploding = true;
        pager.Enabled = false;
        UserData.Instance.UnlockChapter(chapter.Index);
        explodingChapter = new ChapterLocked(chapter.Index, menu)
        {
            TargetChapter = chapter
        };
        chapter.AddChild(explodingChapter);
        explodingChapter.ExplodeEvent.AddListener(delegate
        {
            OnLockExplode(explodingChapter);
        });
        explodingChapter.IgnoreParentOpacity = true;
        chapter.OpacityFloat = 0.001f;
        SetTargetChapter(chapter.Index);
    }

    public void SetTargetChapter(int index, float seconds = 0.3f)
    {
        if (!hasExplodingChapter)
        {
            pager.CurrentPosition = index - 1;
            _ = this.Schedule(0.3f, delegate
            {
                pager.SetTargetPosition(index);
            }, null);
        }
    }

    private void OnLockExplode(ChapterLocked chapter)
    {
        pager.Enabled = true;
        chapter.RemoveListeners();
    }

    private void RefreshPosition()
    {
        for (int i = 0; i < chapters.Count; i++)
        {
            ChapterItem chapterItem = chapters[i];
            float num = Maths.PeriodicOffset(i - currentIndex, ContreJourConstants.PlanetsCount);
            chapterItem.Visible = Math.Abs(num) < 1.5f;
            if (chapterItem.Visible)
            {
                chapterItem.Scale = (chapterItem.Depth = (float)Math.Cos(num)) * 1.5f * PlanetsScale;
                float num3 = chapterItem.Scale * chapterItem.Scale;
                chapterItem.X = (num * 550f) + (AccelerometerOffset.X * 200f * num3);
                chapterItem.Y = AccelerometerOffset.Y * num3 / 4f;
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        pager.Dispose();
        foreach (ChapterItem chapter in chapters)
        {
            chapter.RemoveListeners();
        }
        base.Dispose(disposing);
    }
}
