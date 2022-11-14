namespace ELinkTech.Models;

//*******************************************************************
//Author(s): Jaspreet
//Date: 14 / 11 / 2022
//Perpose:
//Version: 1.0.0
//CopyRight ELinkTech & SoftWe're 2022 (c)
//********************************************************************

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
