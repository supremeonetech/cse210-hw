using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {

        // Create videos
        Video video1 = new Video("Learn C#", "CodeAcademy", 600);
        Video video2 = new Video("Object-Oriented Programming", "TechWorld", 900);
        Video video3 = new Video("Encapsulation Explained", "DevTips", 750);

        // Add comments to video1
        video1.AddComment(new Comment("Alice", "Great tutorial!"));
        video1.AddComment(new Comment("Bob", "Very helpful, thanks!"));
        video1.AddComment(new Comment("Charlie", "Clear explanation."));

        // Add comments to video2
        video2.AddComment(new Comment("Dave", "Loved the examples."));
        video2.AddComment(new Comment("Eve", "Could you make one on inheritance?"));
        video2.AddComment(new Comment("Frank", "Awesome content!"));

        // Add comments to video3
        video3.AddComment(new Comment("Grace", "This was confusing at first, but now I get it."));
        video3.AddComment(new Comment("Heidi", "Thanks for breaking it down."));
        video3.AddComment(new Comment("Ivan", "Please do more videos like this."));

        // Store videos in a list
        List<Video> videos = new List<Video> { video1, video2, video3 };

        // Display video info and comments
        foreach (var video in videos)
        {
            video.DisplayVideoInfo();
        }
    }
}