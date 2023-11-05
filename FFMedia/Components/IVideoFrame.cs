using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMedia.Components;

public interface IVideoFrame : IPictureBufferSpec
{
    bool IsKeyFrame { get; }

    AVPictureType PictureType { get; }

    int CodedPictureNumber { get; }

    int DisplayPictureNumber { get; }

    int RepeatCount { get; }


    bool IsInterlaced { get; }

    bool IsTopFieldFirst { get; }


}
