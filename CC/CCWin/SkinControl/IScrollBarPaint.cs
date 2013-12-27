namespace CCWin.SkinControl
{
    using System;

    public interface IScrollBarPaint
    {
        void OnPaintScrollBarArrow(PaintScrollBarArrowEventArgs e);
        void OnPaintScrollBarThumb(PaintScrollBarThumbEventArgs e);
        void OnPaintScrollBarTrack(PaintScrollBarTrackEventArgs e);
    }
}

