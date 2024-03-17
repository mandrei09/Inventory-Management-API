using MigraDoc.Rendering;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;
using Optima.Fais.Dto;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public class InventoryService : IInventoryService
    {
        private IAppendixAGenerator _appendixAGenerator = null;
		private IAppendixAMinusGenerator _appendixAMinusGenerator = null;
		private IAppendixAPlusGenerator _appendixAPlusGenerator = null;
		private IAllowLabelGenerator _allowLabelGenerator = null;
		private IAppendixBookBeforeInvGenerator _appendixBookBeforeInvGenerator = null;
		private IAppendixBookAfterInvGenerator _appendixBookAfterInvGenerator = null;
		private IAppendixPVGenerator _appendixPVGenerator = null;
		private IAppendixPVFinalGenerator _appendixPVFinalGenerator = null;

		public InventoryService(
            IAppendixAGenerator appendixAGenerator,
            IAppendixAMinusGenerator appendixAMinusGenerator,
            IAppendixAPlusGenerator appendixAPlusGenerator,
            IAllowLabelGenerator allowLabelGenerator,
            IAppendixBookBeforeInvGenerator appendixBookBeforeInvGenerator,
            IAppendixBookAfterInvGenerator appendixBookAfterInvGenerator,
            IAppendixPVGenerator appendixPVGenerator,
            IAppendixPVFinalGenerator appendixPVFinalGenerator)
        {
            _appendixAGenerator = appendixAGenerator;
            _appendixAMinusGenerator = appendixAMinusGenerator;
            _appendixAPlusGenerator = appendixAPlusGenerator;
            _allowLabelGenerator = allowLabelGenerator;
            _appendixBookBeforeInvGenerator = appendixBookBeforeInvGenerator;
            _appendixBookAfterInvGenerator = appendixBookAfterInvGenerator;
            _appendixPVGenerator = appendixPVGenerator;
            _appendixPVFinalGenerator = appendixPVFinalGenerator;
        }


        public async Task<MemoryStream> PreviewAppendixAAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateEnd)
        {
            var document = await this._appendixAGenerator.GenerateDocumentAsync(inventoryId, reportFilter, inventoryDateEnd);

			if(document == null)
			{
				return null;
			}
			else
			{
                /*
                ////// draw rectangle
                //DocumentRenderer docRenderer = new DocumentRenderer(document.Document);
                //docRenderer.PrepareDocument();

                //int pageCount = docRenderer.FormattedDocument.PageCount;
                //RenderInfo[] RenderInfos = docRenderer.GetRenderInfoFromPage(pageCount);
                //RenderInfo r = RenderInfos[RenderInfos.Count() - 1];
                ////int lastElementBottom = (int)(r.LayoutInfo.ContentArea.Y.Millimeter + r.LayoutInfo.ContentArea.Height.Millimeter);
                //double leftOffset = r.LayoutInfo.ContentArea.X.Millimeter;
                //double topOffset = r.LayoutInfo.ContentArea.Y.Millimeter;

                //////PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
                //////renderer.Document = document.Document;

                //////renderer.RenderDocument();
                //////renderer.PdfDocument.Save(filePath);

                ////// end draw rectangle
                */

                PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);


                pdfRenderer.Document = document.Document;
                pdfRenderer.RenderDocument();

                /*
                //var page = pdfRenderer.PdfDocument.Pages[pdfRenderer.PdfDocument.PageCount - 1];
                //XGraphics gfx = XGraphics.FromPdfPage(page);
                //XPen pen = new XPen(XColors.Navy, 1);

                //foreach(var participant in document.Participants)
                //{
                //    gfx.DrawRectangle(pen, Unit.FromMillimeter(participant.SigningArea.Left + leftOffset), Unit.FromMillimeter(participant.SigningArea.Top + topOffset),
                //        Unit.FromMillimeter(participant.SigningArea.Width), Unit.FromMillimeter(participant.SigningArea.Height));
                //}
                */

                MemoryStream ms = new MemoryStream();
                pdfRenderer.PdfDocument.Save(ms, false);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
                ms.Read(buffer, 0, (int)ms.Length);
                return ms;
            }
		}

		public async Task<MemoryStream> PreviewAppendixAMinusAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateEnd)
		{
			var document = await this._appendixAMinusGenerator.GenerateDocumentAsync(inventoryId, reportFilter, inventoryDateEnd);
			if(document == null)
			{
				return null;
			}
			else
			{
                PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
                pdfRenderer.Document = document.Document;
                pdfRenderer.RenderDocument();

                //var page = pdfRenderer.PdfDocument.Pages[pdfRenderer.PdfDocument.PageCount - 1];
                //XGraphics gfx = XGraphics.FromPdfPage(page);
                //XPen pen = new XPen(XColors.Navy, 1);

                //foreach(var participant in document.Participants)
                //{
                //    gfx.DrawRectangle(pen, Unit.FromMillimeter(participant.SigningArea.Left + leftOffset), Unit.FromMillimeter(participant.SigningArea.Top + topOffset),
                //        Unit.FromMillimeter(participant.SigningArea.Width), Unit.FromMillimeter(participant.SigningArea.Height));
                //}

                MemoryStream ms = new MemoryStream();
                pdfRenderer.PdfDocument.Save(ms, false);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
                ms.Read(buffer, 0, (int)ms.Length);
                return ms;
            }
			////// draw rectangle
			//DocumentRenderer docRenderer = new DocumentRenderer(document.Document);
			//docRenderer.PrepareDocument();

			//int pageCount = docRenderer.FormattedDocument.PageCount;
			//RenderInfo[] RenderInfos = docRenderer.GetRenderInfoFromPage(pageCount);
			//RenderInfo r = RenderInfos[RenderInfos.Count() - 1];
			////int lastElementBottom = (int)(r.LayoutInfo.ContentArea.Y.Millimeter + r.LayoutInfo.ContentArea.Height.Millimeter);
			//double leftOffset = r.LayoutInfo.ContentArea.X.Millimeter;
			//double topOffset = r.LayoutInfo.ContentArea.Y.Millimeter;

			//////PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
			//////renderer.Document = document.Document;

			//////renderer.RenderDocument();
			//////renderer.PdfDocument.Save(filePath);

			////// end draw rectangle

			
		}

		public async Task<MemoryStream> PreviewAppendixAPlusAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateEnd)
		{
			var document = await this._appendixAPlusGenerator.GenerateDocumentAsync(inventoryId, reportFilter, inventoryDateEnd);

            if (document == null)
            {
                return null;
            }

			else
			{
                /*
                ////// draw rectangle
                //DocumentRenderer docRenderer = new DocumentRenderer(document.Document);
                //docRenderer.PrepareDocument();

                //int pageCount = docRenderer.FormattedDocument.PageCount;
                //RenderInfo[] RenderInfos = docRenderer.GetRenderInfoFromPage(pageCount);
                //RenderInfo r = RenderInfos[RenderInfos.Count() - 1];
                ////int lastElementBottom = (int)(r.LayoutInfo.ContentArea.Y.Millimeter + r.LayoutInfo.ContentArea.Height.Millimeter);
                //double leftOffset = r.LayoutInfo.ContentArea.X.Millimeter;
                //double topOffset = r.LayoutInfo.ContentArea.Y.Millimeter;

                //////PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
                //////renderer.Document = document.Document;

                //////renderer.RenderDocument();
                //////renderer.PdfDocument.Save(filePath);

                ////// end draw rectangle
                ///*/

                PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
                pdfRenderer.Document = document.Document;
                pdfRenderer.RenderDocument();

                /*
                //var page = pdfRenderer.PdfDocument.Pages[pdfRenderer.PdfDocument.PageCount - 1];
                //XGraphics gfx = XGraphics.FromPdfPage(page);
                //XPen pen = new XPen(XColors.Navy, 1);

                //foreach(var participant in document.Participants)
                //{
                //    gfx.DrawRectangle(pen, Unit.FromMillimeter(participant.SigningArea.Left + leftOffset), Unit.FromMillimeter(participant.SigningArea.Top + topOffset),
                //        Unit.FromMillimeter(participant.SigningArea.Width), Unit.FromMillimeter(participant.SigningArea.Height));
                //}*/

                MemoryStream ms = new MemoryStream();
                pdfRenderer.PdfDocument.Save(ms, false);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
                ms.Read(buffer, 0, (int)ms.Length);
                return ms;
            }
		}

		public async Task<MemoryStream> AllowLabelAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateEnd)
		{
			var document = await this._allowLabelGenerator.GenerateDocumentAsync(inventoryId, reportFilter, inventoryDateEnd);

			if (document == null)
			{
				return null;
			}

			else
			{
                ////// draw rectangle
                //DocumentRenderer docRenderer = new DocumentRenderer(document.Document);
                //docRenderer.PrepareDocument();

                //int pageCount = docRenderer.FormattedDocument.PageCount;
                //RenderInfo[] RenderInfos = docRenderer.GetRenderInfoFromPage(pageCount);
                //RenderInfo r = RenderInfos[RenderInfos.Count() - 1];
                ////int lastElementBottom = (int)(r.LayoutInfo.ContentArea.Y.Millimeter + r.LayoutInfo.ContentArea.Height.Millimeter);
                //double leftOffset = r.LayoutInfo.ContentArea.X.Millimeter;
                //double topOffset = r.LayoutInfo.ContentArea.Y.Millimeter;

                //////PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
                //////renderer.Document = document.Document;

                //////renderer.RenderDocument();
                //////renderer.PdfDocument.Save(filePath);

                ////// end draw rectangle

                PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
                pdfRenderer.Document = document.Document;
                pdfRenderer.RenderDocument();

                //var page = pdfRenderer.PdfDocument.Pages[pdfRenderer.PdfDocument.PageCount - 1];
                //XGraphics gfx = XGraphics.FromPdfPage(page);
                //XPen pen = new XPen(XColors.Navy, 1);

                //foreach(var participant in document.Participants)
                //{
                //    gfx.DrawRectangle(pen, Unit.FromMillimeter(participant.SigningArea.Left + leftOffset), Unit.FromMillimeter(participant.SigningArea.Top + topOffset),
                //        Unit.FromMillimeter(participant.SigningArea.Width), Unit.FromMillimeter(participant.SigningArea.Height));
                //}

                MemoryStream ms = new MemoryStream();
                pdfRenderer.PdfDocument.Save(ms, false);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
                ms.Read(buffer, 0, (int)ms.Length);
                return ms;
            }
		}

		public async Task<MemoryStream> BookBeforeAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateStart)
		{
			var document = await this._appendixBookBeforeInvGenerator.GenerateDocumentAsync(inventoryId, reportFilter, inventoryDateStart);

			if (document == null)
			{
				return null;
			}

			else
			{
                /*
                ////// draw rectangle
                //DocumentRenderer docRenderer = new DocumentRenderer(document.Document);
                //docRenderer.PrepareDocument();

                //int pageCount = docRenderer.FormattedDocument.PageCount;
                //RenderInfo[] RenderInfos = docRenderer.GetRenderInfoFromPage(pageCount);
                //RenderInfo r = RenderInfos[RenderInfos.Count() - 1];
                ////int lastElementBottom = (int)(r.LayoutInfo.ContentArea.Y.Millimeter + r.LayoutInfo.ContentArea.Height.Millimeter);
                //double leftOffset = r.LayoutInfo.ContentArea.X.Millimeter;
                //double topOffset = r.LayoutInfo.ContentArea.Y.Millimeter;

                //////PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
                //////renderer.Document = document.Document;

                //////renderer.RenderDocument();
                //////renderer.PdfDocument.Save(filePath);

                ////// end draw rectangle
                ///*/

                PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
                pdfRenderer.Document = document.Document;
                pdfRenderer.RenderDocument();
                
                /*

                //var page = pdfRenderer.PdfDocument.Pages[pdfRenderer.PdfDocument.PageCount - 1];
                //XGraphics gfx = XGraphics.FromPdfPage(page);
                //XPen pen = new XPen(XColors.Navy, 1);

                //foreach(var participant in document.Participants)
                //{
                //    gfx.DrawRectangle(pen, Unit.FromMillimeter(participant.SigningArea.Left + leftOffset), Unit.FromMillimeter(participant.SigningArea.Top + topOffset),
                //        Unit.FromMillimeter(participant.SigningArea.Width), Unit.FromMillimeter(participant.SigningArea.Height));
                //}*/

                MemoryStream ms = new MemoryStream();
                pdfRenderer.PdfDocument.Save(ms, false);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
                ms.Read(buffer, 0, (int)ms.Length);
                return ms;
            }
		}

		public async Task<MemoryStream> BookAfterAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateEnd)
		{
			var document = await this._appendixBookAfterInvGenerator.GenerateDocumentAsync(inventoryId, reportFilter, inventoryDateEnd);

            if (document == null)
            {
                return null;
            }

            else
            {
                ////// draw rectangle
                //DocumentRenderer docRenderer = new DocumentRenderer(document.Document);
                //docRenderer.PrepareDocument();

                //int pageCount = docRenderer.FormattedDocument.PageCount;
                //RenderInfo[] RenderInfos = docRenderer.GetRenderInfoFromPage(pageCount);
                //RenderInfo r = RenderInfos[RenderInfos.Count() - 1];
                ////int lastElementBottom = (int)(r.LayoutInfo.ContentArea.Y.Millimeter + r.LayoutInfo.ContentArea.Height.Millimeter);
                //double leftOffset = r.LayoutInfo.ContentArea.X.Millimeter;
                //double topOffset = r.LayoutInfo.ContentArea.Y.Millimeter;

                //////PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
                //////renderer.Document = document.Document;

                //////renderer.RenderDocument();
                //////renderer.PdfDocument.Save(filePath);

                ////// end draw rectangle

                PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
                pdfRenderer.Document = document.Document;
                pdfRenderer.RenderDocument();

                //var page = pdfRenderer.PdfDocument.Pages[pdfRenderer.PdfDocument.PageCount - 1];
                //XGraphics gfx = XGraphics.FromPdfPage(page);
                //XPen pen = new XPen(XColors.Navy, 1);

                //foreach(var participant in document.Participants)
                //{
                //    gfx.DrawRectangle(pen, Unit.FromMillimeter(participant.SigningArea.Left + leftOffset), Unit.FromMillimeter(participant.SigningArea.Top + topOffset),
                //        Unit.FromMillimeter(participant.SigningArea.Width), Unit.FromMillimeter(participant.SigningArea.Height));
                //}

                MemoryStream ms = new MemoryStream();
                pdfRenderer.PdfDocument.Save(ms, false);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
                ms.Read(buffer, 0, (int)ms.Length);
                return ms;
            }
		}

		public async Task<MemoryStream> BookPVAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateStart, DateTime? inventoryDateEnd)
		{
			var document = await this._appendixPVGenerator.GenerateDocumentAsync(inventoryId, reportFilter, inventoryDateStart, inventoryDateEnd);

            if (document == null)
            {
                return null;
            }

            else
            {
                PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
                pdfRenderer.Document = document.Document;
                pdfRenderer.RenderDocument();
              
                MemoryStream ms = new MemoryStream();
                pdfRenderer.PdfDocument.Save(ms, false);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
                ms.Read(buffer, 0, (int)ms.Length);
                return ms;
            }   
		}

        public async Task<MemoryStream> BookPVFinalAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateStart, DateTime? inventoryDateEnd)
        {
            var document = await this._appendixPVFinalGenerator.GenerateDocumentAsync(inventoryId, reportFilter, inventoryDateStart, inventoryDateEnd);

            if (document == null)
            {
                return null;
            }

            else
            {
                PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
                pdfRenderer.Document = document.Document;
                pdfRenderer.RenderDocument();

                MemoryStream ms = new MemoryStream();
                pdfRenderer.PdfDocument.Save(ms, false);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
                ms.Read(buffer, 0, (int)ms.Length);
                return ms;
            }
        }
    }
}
