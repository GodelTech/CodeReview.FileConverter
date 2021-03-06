﻿namespace GodelTech.CodeReview.FileConverter.Services
{
    public interface IPathService
    {
        string GetFileName(string path);
        string GetDirectoryName(string path);
        string GetFullPath(string path);
        string Combine(params string[] parts);
    }
}