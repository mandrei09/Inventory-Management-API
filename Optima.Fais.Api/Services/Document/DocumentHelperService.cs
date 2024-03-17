using Microsoft.Extensions.Configuration;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using Optima.Faia.Api.Services;
using Optima.Fais.Dto;
using System.Collections.Generic;

namespace Optima.Fais.Api.Services
{
    public class DocumentHelperService : IDocumentHelperService
    {
        private readonly string _committeePositionPresident = string.Empty;
        private readonly string _committeePositionInternalMember = string.Empty;
        private readonly string _committeePositionExternalMember = string.Empty;
        private readonly string _committeePositionSecretary = string.Empty;
        private readonly string _committeePositionAdministrator = string.Empty;

        public DocumentHelperService(IConfiguration configuration)
        {
            this._committeePositionPresident = configuration.GetSection("CommitteePosition").GetValue<string>("President");
            this._committeePositionSecretary = configuration.GetSection("CommitteePosition").GetValue<string>("Secretary");
            this._committeePositionInternalMember = configuration.GetSection("CommitteePosition").GetValue<string>("InternalMember");
            this._committeePositionExternalMember = configuration.GetSection("CommitteePosition").GetValue<string>("ExternalMember");
            this._committeePositionAdministrator = configuration.GetSection("CommitteePosition").GetValue<string>("Administrator");
        }

        public List<ParticipantDetail> AddSignatureArea(Section section, string tag, List<SigningEmployeeDetail> members, bool landscape = false)
        {
            var participantAreaList = new List<ParticipantDetail>();
            int noOfColumns = landscape ? 3 : 2;

            if ((members == null) || (members.Count == 0)) return participantAreaList;

            //Section section = document.AddSection();

            //section.PageSetup.TopMargin = Unit.FromMillimeter(10);
            //section.PageSetup.LeftMargin = Unit.FromMillimeter(20);
            //section.PageSetup.PageFormat = PageFormat.A4;
            //section.PageSetup.Orientation = Orientation.Landscape;
            //section.PageSetup.HeaderDistance = Unit.FromMillimeter(10);
            //section.PageSetup.BottomMargin = Unit.FromMillimeter(40);

            //section.PageSetup.DifferentFirstPageHeaderFooter = true;
            //section.Tag = tag;

            //List<Employee> members = new List<Employee>();
            //members.Add(administrator);
            //members.AddRange(committeeMembers);
            //members.AddRange(committeeMembers);
            //members.AddRange(committeeMembers);
            //members.AddRange(committeeMembers);

            double currentTop = 0;
            double currentLeft = 0;
            double totalWidth = landscape ? 260 : 180;
            double separatorWidth = 5;
            double separatorHeight = 5;
            double headerRowHeight = 10;
            double signatureRowHeight = 20;
            double missingLineSize = 0;

            //Employee leftEmployee = null;
            //Employee rightEmployee = null;
            //string leftInfo = string.Empty;
            //string rightInfo = string.Empty;
            //bool oneItem = false;

            //int mIndex = 0;
            //int steps = 0;
            //int remainingMembers = members.Count;

            var table = section.AddTable();
            table.Tag = tag;
            double areaWidth;
            AddTableColumnDefinition(table, totalWidth, separatorWidth, missingLineSize, noOfColumns, out areaWidth);
            //table.Borders.Visible = true;
            //table.Borders.Color = Color.FromRgb(255, 255, 0);
            double itemWidth = areaWidth + areaWidth;

            Row missingLineRow = null;
            Row headerRow = null;
            Row signatureRow = null;
            int columnIndex = 0;

            for (int memberIndex = 0; memberIndex < members.Count; memberIndex++)
            {
                var member = members[memberIndex];

                int indexInLine = memberIndex % noOfColumns;
                if (indexInLine == 0)
                {
                    currentLeft = 0;

                    if (memberIndex > 0)
                    {
                        missingLineRow = table.AddRow();
                        missingLineRow.HeightRule = RowHeightRule.Exactly;
                        missingLineRow.Height = Unit.FromMillimeter(missingLineSize);
                        missingLineRow.VerticalAlignment = VerticalAlignment.Center;

                        currentTop += (signatureRowHeight + separatorHeight + missingLineSize);
                    }

                    headerRow = table.AddRow();
                    headerRow.HeightRule = RowHeightRule.Exactly;
                    headerRow.Height = Unit.FromMillimeter(headerRowHeight);
                    headerRow.VerticalAlignment = VerticalAlignment.Center;

                    signatureRow = table.AddRow();
                    signatureRow.HeightRule = RowHeightRule.Exactly;
                    signatureRow.Height = Unit.FromMillimeter(signatureRowHeight);
                    signatureRow.VerticalAlignment = VerticalAlignment.Center;

                    Row emptySpaceRow = table.AddRow();
                    emptySpaceRow.Borders.Visible = false;
                    emptySpaceRow.HeightRule = RowHeightRule.Exactly;
                    emptySpaceRow.Height = Unit.FromMillimeter(separatorHeight);
                    emptySpaceRow.VerticalAlignment = VerticalAlignment.Center;

                    columnIndex = 0;
                    currentTop += headerRowHeight;
                }

                if (missingLineRow != null) missingLineRow.Cells[columnIndex].Borders.Visible = true;

                if (columnIndex > 0)
                {
                    headerRow.Cells[columnIndex - 1].Borders.Visible = true;
                    signatureRow.Cells[columnIndex - 1].Borders.Visible = true;
                }

                headerRow.Cells[columnIndex].AddParagraph("Semnătură:");
                headerRow.Cells[columnIndex].Format.Alignment = ParagraphAlignment.Center;
                headerRow.Cells[columnIndex].Format.Font.Size = 8;
                headerRow.Cells[columnIndex].Format.Font.Bold = true;
                headerRow.Cells[columnIndex].Borders.Top.Visible = true;
                headerRow.Cells[columnIndex].Borders.Visible = true;

                signatureRow.Cells[columnIndex].Borders.Visible = true;

                columnIndex++;
                if (missingLineRow != null) missingLineRow.Cells[columnIndex].Borders.Visible = true;
                headerRow.Cells[columnIndex].AddParagraph(member.InvCommitteePosition.Name);
                headerRow.Cells[columnIndex].Format.Alignment = ParagraphAlignment.Center;
                headerRow.Cells[columnIndex].Format.Font.Size = 8;
                headerRow.Cells[columnIndex].Format.Font.Bold = true;
                headerRow.Cells[columnIndex].Borders.Visible = true;

                signatureRow.Cells[columnIndex].AddParagraph($"{member.Employee.FirstName} {member.Employee.LastName}");
                signatureRow.Cells[columnIndex].Format.Alignment = ParagraphAlignment.Center;
                signatureRow.Cells[columnIndex].Format.Font.Size = 8;
                signatureRow.Cells[columnIndex].Format.Font.Bold = true;
                signatureRow.Cells[columnIndex].Borders.Visible = true;

                var signingArea = new SigningArea();
                if (landscape)
                {
                    signingArea.LTop = currentTop;
                    signingArea.LLeft = currentLeft;
                    signingArea.LHeight = signatureRowHeight;
                    signingArea.LWidth = areaWidth;
                }
                else
                {
                    signingArea.Top = currentTop;
                    signingArea.Left = currentLeft;
                    signingArea.Height = signatureRowHeight;
                    signingArea.Width = areaWidth;
                }

                participantAreaList.Add(new ParticipantDetail { Email = member.Employee.Email, SigningArea = signingArea }); // new SigningArea { Top = currentTop, Left = currentLeft, Height = signatureRowHeight, Width = areaWidth } });

                columnIndex += 3;
                currentLeft = currentLeft + itemWidth + separatorWidth;
            }

            //while (remainingMembers > 0)
            //{
            //    steps++;

            //    leftEmployee = members[mIndex];
            //    if (remainingMembers == 1)
            //    {
            //        oneItem = true;
            //        rightEmployee = null;
            //        mIndex += 1;
            //        remainingMembers -= 1;
            //    }
            //    else
            //    {
            //        oneItem = false;
            //        rightEmployee = members[mIndex + 1];
            //        mIndex += 2;
            //        remainingMembers -= 2;
            //    }

            //    //if (signatureAreaFirstRow == null)
            //    //{
            //    //    signatureAreaFirstRow = table.AddRow();
            //    //    signatureAreaFirstRow.Borders.Visible = false;
            //    //    signatureAreaFirstRow.HeightRule = RowHeightRule.Exactly;
            //    //    signatureAreaFirstRow.Height = Unit.FromMillimeter(headerRowHeight);
            //    //    signatureAreaFirstRow.VerticalAlignment = VerticalAlignment.Center;
            //    //}

            //    var missingLineRow = table.AddRow();
            //    missingLineRow.HeightRule = RowHeightRule.Exactly;
            //    missingLineRow.Height = Unit.FromMillimeter(missingLineSize);
            //    missingLineRow.VerticalAlignment = VerticalAlignment.Center;

            //    var headerRow = table.AddRow();
            //    headerRow.HeightRule = RowHeightRule.Exactly;
            //    headerRow.Height = Unit.FromMillimeter(headerRowHeight);
            //    headerRow.VerticalAlignment = VerticalAlignment.Center;
            //    int index = 0; // oneItem ? 2 : 0;

            //    missingLineRow.Cells[index].Borders.Visible = true;
            //    headerRow.Cells[index].AddParagraph("Semnătură:");
            //    headerRow.Cells[index].Format.Alignment = ParagraphAlignment.Center;
            //    headerRow.Cells[index].Format.Font.Size = 8;
            //    headerRow.Cells[index].Format.Font.Bold = true;
            //    headerRow.Cells[index].Borders.Visible = true;

            //    index++;
            //    missingLineRow.Cells[index].Borders.Visible = true;
            //    headerRow.Cells[index].AddParagraph(mIndex <= 2 ? administratorInfo : memberInfo);
            //    headerRow.Cells[index].Format.Alignment = ParagraphAlignment.Center;
            //    headerRow.Cells[index].Format.Font.Size = 8;
            //    headerRow.Cells[index].Format.Font.Bold = true;
            //    headerRow.Cells[index].Borders.Visible = true;

            //    if (!oneItem)
            //    {
            //        index += 2;
            //        missingLineRow.Cells[index].Borders.Visible = true;
            //        headerRow.Cells[index].Borders.Visible = true;
            //        index++;
            //        missingLineRow.Cells[index].Borders.Visible = true;
            //        headerRow.Cells[index].AddParagraph("Semnătură:");
            //        headerRow.Cells[index].Format.Alignment = ParagraphAlignment.Center;
            //        headerRow.Cells[index].Format.Font.Size = 8;
            //        headerRow.Cells[index].Format.Font.Bold = true;
            //        headerRow.Cells[index].Borders.Visible = true;

            //        index++;
            //        missingLineRow.Cells[index].Borders.Visible = true;
            //        headerRow.Cells[index].AddParagraph(memberInfo);
            //        headerRow.Cells[index].Format.Alignment = ParagraphAlignment.Center;
            //        headerRow.Cells[index].Format.Font.Size = 8;
            //        headerRow.Cells[index].Format.Font.Bold = true;
            //        headerRow.Cells[index].Borders.Visible = true;
            //    }

            //    currentTop += missingLineRow.Height.Millimeter;
            //    currentTop += headerRow.Height.Millimeter;

            //    var signatureRow = table.AddRow();
            //    signatureRow.HeightRule = RowHeightRule.Exactly;
            //    signatureRow.Height = Unit.FromMillimeter(signatureRowHeight);
            //    signatureRow.VerticalAlignment = VerticalAlignment.Center;

            //    if (oneItem)
            //    {
            //        index = 3;
            //        currentLeft = offsetWidth;
            //    }
            //    else
            //    {
            //        index = 1;
            //        currentLeft = 0;
            //    }

            //    index = 0;
            //    currentLeft = 0;

            //    signatureRow.Cells[index].Borders.Visible = true;
            //    index++;
            //    signatureRow.Cells[index].AddParagraph($"{leftEmployee.FirstName} {leftEmployee.LastName}");
            //    signatureRow.Cells[index].Format.Alignment = ParagraphAlignment.Center;
            //    signatureRow.Cells[index].Format.Font.Size = 8;
            //    signatureRow.Cells[index].Format.Font.Bold = true;
            //    signatureRow.Cells[index].Borders.Visible = true;
            //    participantAreaList.Add(new ParticipantDetail { Email = leftEmployee.Email, SigningArea = new SigningArea { Top = currentTop, Left = currentLeft, Height = signatureRowHeight, Width = areaWidth } });

            //    if (!oneItem)
            //    {
            //        index += 2;
            //        signatureRow.Cells[index].Borders.Visible = true;
            //        index++;
            //        signatureRow.Cells[index].Borders.Visible = true;
            //        index++;
            //        signatureRow.Cells[index].AddParagraph($"{rightEmployee.FirstName} {rightEmployee.LastName}");
            //        signatureRow.Cells[index].Format.Alignment = ParagraphAlignment.Center;
            //        signatureRow.Cells[index].Format.Font.Size = 8;
            //        signatureRow.Cells[index].Format.Font.Bold = true;
            //        signatureRow.Cells[index].Borders.Visible = true;
            //        currentLeft = (2 * areaWidth) + separatorWidth;
            //        participantAreaList.Add(new ParticipantDetail { Email = rightEmployee.Email, SigningArea = new SigningArea { Top = currentTop, Left = currentLeft, Height = signatureRowHeight, Width = areaWidth } });
            //    }

            //    currentTop += signatureRow.Height.Millimeter;

            //    var emptySpaceRow = table.AddRow();
            //    emptySpaceRow.Borders.Visible = false;
            //    emptySpaceRow.HeightRule = RowHeightRule.Exactly;
            //    emptySpaceRow.Height = Unit.FromMillimeter(separatorHeight);
            //    emptySpaceRow.VerticalAlignment = VerticalAlignment.Center;

            //    currentTop += emptySpaceRow.Height.Millimeter;
            //}

            //signatureAreaFirstRow.KeepWith = (steps * 3);
            return participantAreaList;
        }

        public List<ParticipantDetail> AddSignatureAreaCommitteeMember(Section section, string tag, List<SigningEmployeeDetail> members, bool landscape = false)
        {
            var participantAreaList = new List<ParticipantDetail>();
            int noOfColumns = landscape ? 3 : 2;

            if ((members == null) || (members.Count == 0)) return participantAreaList;

            double currentTop = 0;
            double currentLeft = 0;
            double totalWidth = landscape ? 260 : 180;
            double separatorWidth = 5;
            double separatorHeight = 5;
            double headerRowHeight = 10;
            double signatureRowHeight = 20;
            double missingLineSize = 0;

            var table = section.AddTable();
            table.Tag = tag;
            double areaWidth;
            AddTableColumnDefinition(table, totalWidth, separatorWidth, missingLineSize, noOfColumns, out areaWidth);
            double itemWidth = areaWidth + areaWidth;

            Row missingLineRow = null;
            Row headerRow = null;
            Row signatureRow = null;
            int columnIndex = 0;

            for (int memberIndex = 0; memberIndex < members.Count; memberIndex++)
            {
                var member = members[memberIndex];

                int indexInLine = memberIndex % noOfColumns;
                if (indexInLine == 0)
                {
                    currentLeft = 0;

                    if (memberIndex > 0)
                    {
                        missingLineRow = table.AddRow();
                        missingLineRow.HeightRule = RowHeightRule.Exactly;
                        missingLineRow.Height = Unit.FromMillimeter(missingLineSize);
                        missingLineRow.VerticalAlignment = VerticalAlignment.Center;

                        currentTop += (signatureRowHeight + separatorHeight + missingLineSize);
                    }

                    headerRow = table.AddRow();
                    headerRow.HeightRule = RowHeightRule.Exactly;
                    headerRow.Height = Unit.FromMillimeter(headerRowHeight);
                    headerRow.VerticalAlignment = VerticalAlignment.Center;

                    signatureRow = table.AddRow();
                    signatureRow.HeightRule = RowHeightRule.Exactly;
                    signatureRow.Height = Unit.FromMillimeter(signatureRowHeight);
                    signatureRow.VerticalAlignment = VerticalAlignment.Center;

                    Row emptySpaceRow = table.AddRow();
                    emptySpaceRow.Borders.Visible = false;
                    emptySpaceRow.HeightRule = RowHeightRule.Exactly;
                    emptySpaceRow.Height = Unit.FromMillimeter(separatorHeight);
                    emptySpaceRow.VerticalAlignment = VerticalAlignment.Center;

                    columnIndex = 0;
                    currentTop += headerRowHeight;
                }

                if (member.Employee != null)
                {
                    if (missingLineRow != null) missingLineRow.Cells[columnIndex].Borders.Visible = true;

                    if (columnIndex > 0)
                    {
                        headerRow.Cells[columnIndex - 1].Borders.Visible = true;
                        signatureRow.Cells[columnIndex - 1].Borders.Visible = true;
                    }

                    headerRow.Cells[columnIndex].AddParagraph("Semnătură:");
                    headerRow.Cells[columnIndex].Format.Alignment = ParagraphAlignment.Center;
                    headerRow.Cells[columnIndex].Format.Font.Size = 8;
                    headerRow.Cells[columnIndex].Format.Font.Bold = true;
                    headerRow.Cells[columnIndex].Borders.Top.Visible = true;
                    headerRow.Cells[columnIndex].Borders.Visible = true;

                    signatureRow.Cells[columnIndex].Borders.Visible = true;

                    columnIndex++;
                    if (missingLineRow != null) missingLineRow.Cells[columnIndex].Borders.Visible = true;
                    headerRow.Cells[columnIndex].AddParagraph(member.InvCommitteePosition != null ? member.InvCommitteePosition.Name : member.Info);
                    headerRow.Cells[columnIndex].Format.Alignment = ParagraphAlignment.Center;
                    headerRow.Cells[columnIndex].Format.Font.Size = 8;
                    headerRow.Cells[columnIndex].Format.Font.Bold = true;
                    headerRow.Cells[columnIndex].Borders.Visible = true;

                    string employeeDetails = string.Empty;
                    if (member != null) employeeDetails = $"\r\n\r\n{member.Employee.FirstName} {member.Employee.LastName}";
                    else employeeDetails = $"{member.Employee.FirstName} {member.Employee.LastName}";
                    signatureRow.Cells[columnIndex].AddParagraph(employeeDetails);
                    signatureRow.Cells[columnIndex].Format.Alignment = ParagraphAlignment.Center;
                    signatureRow.Cells[columnIndex].Format.Font.Size = 8;
                    signatureRow.Cells[columnIndex].Format.Font.Bold = true;
                    signatureRow.Cells[columnIndex].Borders.Visible = true;

                    var signingArea = new SigningArea();
                    if (landscape)
                    {
                        signingArea.LTop = currentTop;
                        signingArea.LLeft = currentLeft;
                        signingArea.LHeight = signatureRowHeight;
                        signingArea.LWidth = areaWidth;
                    }
                    else
                    {
                        signingArea.Top = currentTop;
                        signingArea.Left = currentLeft;
                        signingArea.Height = signatureRowHeight;
                        signingArea.Width = areaWidth;
                    }

                    participantAreaList.Add(new ParticipantDetail { Email = member.Employee.Email, SigningArea = signingArea });
                }

                columnIndex += 3;
                currentLeft = currentLeft + itemWidth + separatorWidth;
            }

            return participantAreaList;
        }

        private static void AddTableColumnDefinition(Table table, double totalWidth, double separatorWidth, double missingLineSize, int noOfColumns, out double areaWidth)
        {
            Column currentColumn = null;
            bool borders = false;

            areaWidth = (totalWidth - ((noOfColumns - 1) * separatorWidth)) / (noOfColumns + noOfColumns);
            int noOfSeparators = noOfColumns - 1;

            for (int columnIndex = 0; columnIndex < noOfColumns; columnIndex++)
            {
                if (columnIndex > 0)
                {
                    currentColumn = table.AddColumn(Unit.FromMillimeter(missingLineSize));
                    currentColumn.Borders.Visible = borders;
                }

                currentColumn = table.AddColumn(Unit.FromMillimeter(areaWidth));
                currentColumn.Borders.Visible = borders;
                currentColumn = table.AddColumn(Unit.FromMillimeter(areaWidth));
                currentColumn.Borders.Visible = borders;

                if (columnIndex < noOfSeparators)
                {
                    currentColumn = table.AddColumn(Unit.FromMillimeter(separatorWidth));
                    currentColumn.Borders.Visible = borders;
                }
            }
        }
    }
}
