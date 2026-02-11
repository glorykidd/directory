using Directory.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Directory.Web.Helpers;

public static class PdfGenerator
{
    public static byte[] GenerateMemberPdf(MemberDetailDto member)
    {
        using var stream = new MemoryStream();
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Text("CGBC Member Directory").Bold().FontSize(16)
                    .FontColor(Colors.Blue.Medium);

                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text(member.DisplayName).Bold().FontSize(14);
                    col.Item().PaddingTop(5);

                    if (!string.IsNullOrEmpty(member.MaritalStatus))
                        col.Item().Text($"Marital Status: {member.MaritalStatus}");

                    if (!string.IsNullOrEmpty(member.FormattedAddress))
                    {
                        col.Item().PaddingTop(5).Text("Address:").Bold();
                        col.Item().Text(member.FormattedAddress);
                    }

                    if (!string.IsNullOrEmpty(member.CellPhone))
                        col.Item().Text($"Cell: {member.CellPhone}");
                    if (!string.IsNullOrEmpty(member.HomePhone))
                        col.Item().Text($"Home: {member.HomePhone}");
                    if (!string.IsNullOrEmpty(member.Email1))
                        col.Item().Text($"Email: {member.Email1}");
                    if (!string.IsNullOrEmpty(member.Email2))
                        col.Item().Text($"Email 2: {member.Email2}");
                    if (!string.IsNullOrEmpty(member.DateOfBirth))
                        col.Item().Text($"Date of Birth: {member.DateOfBirth}");
                    if (!string.IsNullOrEmpty(member.MarriageDate))
                        col.Item().Text($"Marriage Date: {member.MarriageDate}");

                    if (member.RelatedMembers.Count > 0)
                    {
                        col.Item().PaddingTop(10).Text("Related Members:").Bold();
                        foreach (var rel in member.RelatedMembers)
                        {
                            col.Item().Text($"  {rel.RelationshipTypeName}: {rel.DisplayName}");
                        }
                    }

                    if (member.Notes.Count > 0)
                    {
                        col.Item().PaddingTop(10).Text("Notes:").Bold();
                        foreach (var note in member.Notes)
                        {
                            col.Item().Text($"  [{note.CreatedDate.ToShortDateString()}] {note.NoteText}");
                        }
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generated on ");
                    text.Span(DateTime.Now.ToShortDateString());
                });
            });
        }).GeneratePdf(stream);

        return stream.ToArray();
    }
}
