using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Reflection;
using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Trello.Models;

using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using FontSize = DocumentFormat.OpenXml.Wordprocessing.FontSize;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using Type = Trello.Models.Type;


namespace Trello.Utils
{
    public class WordExtraction
    {
        private static readonly string[] ImageExtensions = { ".jpeg", ".jpg", ".bmp", ".png" };

        public static void DocumentWithOpenXml(MemoryStream memoryStream, Board board)
        {
            var titleRunProperties = AddRunProperties("Arial", "56", true, false);
            var headingRunProperties = AddRunProperties("Arial", "32", true, false);
            var cardRunProperties = AddRunProperties("Arial", "24", true, false);
            var paragraphRunProperties = AddRunProperties("Calibri", "22", false, false);
            var commentDateRunProperties = AddRunProperties("Calibri", "20", false, false);

            using (var document = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
            {
                // Add a new main document part. 
                var mainPart = document.AddMainDocumentPart();

                // Create DOM tree for simple document. 
                mainPart.Document = new Document();
                AddDefaultWordStyles(mainPart);

                var body = new Body();
                var title = CreateTitleParagraph(board.Name, titleRunProperties);
                body.AppendChild(title);

                var date = CreateDateParagraph(paragraphRunProperties);
                body.AppendChild(date);

                // Append board's Lists and cards
                if (board.Lists != null)
                {
                    foreach (var list in board.Lists)
                    {
                        var listName = AddParagraph(headingRunProperties, list.Name, "Heading1");
                        body.AppendChild(listName);

                        foreach (var card in list.Cards)
                        {
                            // Card Name.
                            var cardName = AddParagraph(cardRunProperties, card.Name, "Heading2");
                            body.AppendChild(cardName);

                            // Card Attachments
                            foreach (var attachment in card.Attachments)
                            {
                                var attachmentUrl = attachment.Url;
                                if (!IsImage(attachmentUrl))
                                {
                                    continue;
                                }

                                var imageParagraph = InsertImage(mainPart, attachmentUrl);
                                body.AppendChild(imageParagraph);
                            }

                            // Card text.
                            var cardText = AddParagraph(paragraphRunProperties, card.Description, null);
                            body.AppendChild(cardText);

                            // Card comments (Actions).
                            if (card.Actions != null)
                            {
                                foreach (var action in card.Actions)
                                {
                                    if (action.Type != Type.commentCard) continue;

                                    var fullName = string.Format("Comment by {0}", action.MemberCreator.FullName);
                                    var spacing = new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" };
                                    var commentAuthor = AddParagraph(cardRunProperties, fullName, null);
                                    commentAuthor.ParagraphProperties.PrependChild(spacing);

                                    var newDate = DateTime.Parse(action.Date, null, DateTimeStyles.RoundtripKind);
                                    var commentDate = AddParagraph(commentDateRunProperties, newDate.ToString("g"), null);

                                    var paragraphText = action.Data.Text;
                                    var commentText = AddParagraph(paragraphRunProperties, paragraphText, null);

                                    body.AppendChild(commentAuthor);
                                    body.AppendChild(commentDate);
                                    body.AppendChild(commentText);
                                }
                            }
                        }
                    }
                }

                mainPart.Document.AppendChild(body);
            }
        }

        private static bool IsImage(string attachmentUrl)
        {
            return ImageExtensions.Any(attachmentUrl.EndsWith);
        }

        private static Paragraph InsertImage(MainDocumentPart mainPart, string imagePath)
        {
            var extension = Path.GetExtension(imagePath);
            var imagePart = AddImagePart(mainPart, extension);

            byte[] imageBytes;
            using (var webClient = new WebClient())
            {
                imageBytes = webClient.DownloadData(imagePath);
            }

            long widthEmus;
            long heightEmus;
            using (var stream = new MemoryStream(imageBytes))
            {
                var image = Image.FromStream(stream);
                SetImageDimensions(image, out widthEmus, out heightEmus);
            }

            using (var stream = new MemoryStream(imageBytes))
            {
                imagePart.FeedData(stream);
            }

            var idOfPart = mainPart.GetIdOfPart(imagePart);
            var paragraph = CreateImageParagraph(idOfPart, widthEmus, heightEmus);

            return paragraph;
        }

        private static ImagePart AddImagePart(MainDocumentPart mainPart, string extension)
        {
            ImagePart imagePart;
            switch (extension)
            {
                case ".png":
                    imagePart = mainPart.AddImagePart(ImagePartType.Png);
                    break;
                case ".jpeg":
                    imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
                    break;
                case ".jpg":
                    imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
                    break;
                case ".bmp":
                    imagePart = mainPart.AddImagePart(ImagePartType.Bmp);
                    break;
                default:
                    imagePart = mainPart.AddImagePart(ImagePartType.Bmp);
                    break;
            }
            return imagePart;
        }

        private static Paragraph CreateImageParagraph(string relationshipId, long widthEmus, long heightEmus)
        {
            // Define the reference of the image.
            var element =
                new Drawing(
                    new DW.Inline(
                        new DW.Extent() { Cx = widthEmus, Cy = heightEmus },
                        new DW.EffectExtent()
                        {
                            LeftEdge = 0L,
                            TopEdge = 0L,
                            RightEdge = 0L,
                            BottomEdge = 0L
                        },
                        new DW.DocProperties() { Id = (UInt32Value)1U, Name = "Picture 1" },
                        new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoChangeAspect = true }),
                        new A.Graphic(
                            new A.GraphicData(
                                new PIC.Picture(
                                    new PIC.NonVisualPictureProperties(
                                        new PIC.NonVisualDrawingProperties()
                                        {
                                            Id = (UInt32Value)0U,
                                            Name = "New Bitmap Image.jpg"
                                        },
                                        new PIC.NonVisualPictureDrawingProperties()),
                                    new PIC.BlipFill(
                                        new A.Blip(
                                            new A.BlipExtensionList(
                                                new A.BlipExtension()
                                                {
                                                    Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                })
                                            )
                                        {
                                            Embed = relationshipId,
                                            CompressionState = A.BlipCompressionValues.Print
                                        },
                                        new A.Stretch(
                                            new A.FillRectangle())),
                                    new PIC.ShapeProperties(
                                        new A.Transform2D(
                                            new A.Offset() { X = 0L, Y = 0L },
                                            new A.Extents() { Cx = widthEmus, Cy = heightEmus }), // 990000L  792000L  default values
                                        new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }))) { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }))
                    {
                        DistanceFromTop = (UInt32Value)0U,
                        DistanceFromBottom = (UInt32Value)0U,
                        DistanceFromLeft = (UInt32Value)0U,
                        DistanceFromRight = (UInt32Value)0U,
                        EditId = "50D07946"
                    });

            return new Paragraph(new Run(element));
        }

        private static void SetImageDimensions(Image image, out long widthEmus, out long heightEmus)
        {
            // Setting image dimensions
            var widthPx = image.Width;
            var heightPx = image.Height;
            var horzRezDpi = image.HorizontalResolution;
            var vertRezDpi = image.VerticalResolution;

            // EMUs (English Metric Unit)
            const int emusPerInch = 914400;
            const int emusPerCm = 360000;
            var maxWidthCm = 16.51;
            var maxHeightCm = 23.70;
            widthEmus = (long)(widthPx / horzRezDpi * emusPerInch);
            heightEmus = (long)(heightPx / vertRezDpi * emusPerInch);
            var maxWidthEmus = (long)(maxWidthCm * emusPerCm);
            var maxHeightEmus = (long)(maxHeightCm * emusPerCm);

            // Limiting excess width
            if (widthEmus > maxWidthEmus)
            {
                var ratio = (heightEmus * 1.0m) / widthEmus;
                widthEmus = maxWidthEmus;
                heightEmus = (long)(widthEmus * ratio);
            }

            // Limiting excess height
            if (heightEmus > maxHeightEmus)
            {
                var ratio = (widthEmus * 1.0m) / heightEmus;
                heightEmus = maxHeightEmus;
                widthEmus = (long)(heightEmus * ratio);
            }
        }

        private static void AddDefaultWordStyles(MainDocumentPart mainPart)
        {
            var part = mainPart.AddNewPart<StyleDefinitionsPart>();
            part.Styles = new Styles();

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Trello.Utils.Resources.Default.dotx";
            var stream = assembly.GetManifestResourceStream(resourceName);
            // ReSharper disable once AssignNullToNotNullAttribute
            using (stream)
            using (var wordTemplate = WordprocessingDocument.Open(stream, false))
            {
                foreach (var templateStyle in wordTemplate.MainDocumentPart.StyleDefinitionsPart.Styles)
                {
                    part.Styles.AppendChild(templateStyle.CloneNode(true));
                }
            }
        }

        private static Paragraph CreateTitleParagraph(string titleText, RunProperties paragraphProperties)
        {
            var title = AddParagraph(paragraphProperties, titleText, null);
            return title;
        }

        private static Paragraph CreateDateParagraph(RunProperties runProperties)
        {
            var date = DateTime.Today;
            var dateParagraph = AddParagraph(runProperties, date.ToShortDateString(), null);
            return dateParagraph;
        }

        private static Paragraph AddParagraph(RunProperties runProperties, string paragraphText, string headingStyle)
        {
            var paragraph = new Paragraph();

            // Append elements appropriately. 
            var paragraphProperties = GetJustifiedParagraphProperties(headingStyle);
            paragraph.PrependChild(paragraphProperties);

            var paragraphRun = CreateParagraphRun(runProperties, paragraphText);
            paragraph.AppendChild(paragraphRun);

            return paragraph;
        }

        private static Run CreateParagraphRun(RunProperties runProperties, string paragraphText)
        {
            var run = new Run();
            run.AppendChild(runProperties.CloneNode(true));
            ParseTextForOpenXml(run, paragraphText);

            return run;
        }

        private static void ParseTextForOpenXml(Run run, string textualData)
        {
            var textArray = textualData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            var first = true;

            foreach (var line in textArray)
            {
                if (!first)
                {
                    run.Append(new Break());
                }

                first = false;

                var txt = new Text { Text = line };
                run.Append(txt);
            }
        }

        private static ParagraphProperties GetJustifiedParagraphProperties(string heading)
        {
            var paragraphProperties = new ParagraphProperties();

            if (heading != null)
            {
                var paragraphStyleId = new ParagraphStyleId() { Val = heading };
                paragraphProperties.AppendChild(paragraphStyleId);
            }

            var justification = new Justification { Val = JustificationValues.Left };
            paragraphProperties.AppendChild(justification);

            return paragraphProperties;
        }

        private static RunProperties AddRunProperties(string textFont, string textSize, bool bold, bool italic)
        {
            var runProperties = new RunProperties();
            var font = new RunFonts { Ascii = textFont };
            var size = new FontSize { Val = new StringValue(textSize) };

            runProperties.AppendChild(font);
            runProperties.AppendChild(size);

            if (bold)
            {
                runProperties.AppendChild(new Bold());
            }

            if (italic)
            {
                runProperties.AppendChild(new Italic());
            }

            return runProperties;
        }
    }
}
