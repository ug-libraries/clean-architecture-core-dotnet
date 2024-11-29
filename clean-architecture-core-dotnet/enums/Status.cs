// <copyright file="Status.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

namespace Ug.Enums
{
    public enum Status
    {
        Success = 1,
        Error = 2,
    }

    public static class StatusExtensions
    {
        public static string GetValue(this Status status)
        {
            switch (status)
            {
                case Status.Success:
                    return "success";
                case Status.Error:
                    return "error";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string ToStringValue(this Status status)
        {
            return status.GetValue();
        }
    }
}