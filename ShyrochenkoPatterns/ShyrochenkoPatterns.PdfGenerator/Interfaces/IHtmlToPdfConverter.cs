using System;
using System.Collections.Generic;
using System.Text;

namespace ShyrochenkoPatterns.PdfGenerator.Interfaces
{
    public interface IHtmlToPdfConverter
    {
        byte[] ConvertHtmlToPdf(string html);
    }
}
