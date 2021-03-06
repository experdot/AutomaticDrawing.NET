﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    public interface IRecognition
    {
        IPainting Recognize(PixelData pixels);
    }
}