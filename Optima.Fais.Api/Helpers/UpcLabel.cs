using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace Optima.Fais.Api.Helpers
{
    public class UpcLabel
    {

        private string invNo;
        private string description;
        private string purchaseDate;
        private string serialNumber;
        private string Cold;
        private string NewMember;
        public UpcLabel()
        {
        }
        public UpcLabel(string invNo, string description, string purchaseDate, string serialNumber, string Cold, string NewMember)
        {
            if (invNo == null && description == null && purchaseDate == "" && serialNumber == "0")
            {
                throw new ArgumentNullException("strFirstName");
            }

            this.invNo = invNo;
            this.description = description;
            this.purchaseDate = purchaseDate;
            this.serialNumber = serialNumber;
            this.Cold = Cold;
            this.NewMember = NewMember;
        }
        public void PrintBarcode(string printerName, string pProductName, string pBarcode, string strNumOfCopies)
        {
            if (printerName == null)
            {
                throw new ArgumentNullException("printerName");
            }
            StringBuilder strBldr = new StringBuilder();
            //strBldr.AppendLine("^XA");

            //strBldr.AppendLine("^FO40,100");
            //strBldr.AppendLine("^AQ,50,30");
            //// sb1.AppendLine("^FDAnja^FS");
            //strBldr.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", pProductName));
            ////^FO100,100^BY3
            //strBldr.AppendLine("^FO80,200^BY3");
            ////^BCN,150,Y,N,Y,N
            //strBldr.AppendLine("^BCN,125,Y,N,Y,N");
            //strBldr.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", pBarcode));
            //strBldr.AppendLine(string.Format(CultureInfo.InvariantCulture, "^PQ{0}", strNumOfCopies));
            //strBldr.AppendLine("^XZ");


            strBldr.Append("^XA^FO10,10^GFA,9880,9880,38,gW0FFC,gU01KF8,gU0MF8,gT07NF,gS01OFC,gS03PF,gS0QFC,gH04O01QFE,gH08O03RF,gG01P0SFCM01,gG02O01SFEM018,gG06O03TFN0C,gG0CO03TF8M06,g018O07TFCM07,g038O0UFEM038,g0FO01VFM03C,Y01EO03VFM01E,Y03EO03VF8L01F,Y07CO07VFCM0F8,Y0FCO07VFCM0FC,X01F8O0WFEM0FE,X03FO01WFEM07F,X07FO01XFM07F8,X0FEO03XFM03FC,W01FEO03XF8L03FE,W03FCO07XF8L03FF,W07FCO07XF8L01FF8,V01FF8O0YFCL01FFC,V03FF8O0YFCL01FFE,V07FFP0XFO0IF,V0IFO01WFP0IF8,U01IFO01VF8P0IFC,U03FFEO03UFEQ0IFE,U07FFEO03UF8Q07IF,U0IFCO03UFR07IF8,T01IFCO07TFER07IFC,T03IFCO07TFCR07IFE,T07IF8O07TF8R07JF,T0JF8O07TF8R07JF8,S01JFP0UFS03JFC,S03JFP0TFES03KF,S0KFP0TFES03KF8,R01JFEO01TFCS03KFC,R03JFEO01TFCS03KFE,R07JFEO01TFCS03LF,R0KFEO01TF8S03LF8,Q01KFCO03TF8S03LFC,Q03KFCO03TF8S03LFE,Q07KFCO03TF8S03MF,Q0LF8O03TF8S03MF8,P01LF8O03TF8S03MFC,P03LF8O07TFT03MFE,P07LF8O07TFT03NF,P0MFP07TFT03NF8,O03MFP07TFT07NFC,O07MFP07TFT07NFE,O0NFP0UFT07OF,N01NFP0UFT07OF8,N03MFEP0UF8S07OFC,N07MFEP0UF8S0PFE,N0NFEP0UF8S0QF,M01NFEP0UF8S0QF8,M03NFEP0UF8R01QFC,M07NFCO01UFCR01QFE,M0OFCO01UFCR03RF,L01OFCO01UFCR03RF8,L03OFCO01UFER07RFC,L07OFCO01UFER0SFE,K01PFCO01VFQ01TF,K03PFCO01VF8P03TF8,K07PF8O01VF8P03TFC,K0QF8O01VFCP0UFE,J01QF8O03VFEO01VF,J03QF8O03WFO03VF8,J07QF8O03WF8N0WFC,J0RF8O03WFEM03WFE,I01RF8O03XFM0YF,I03RF8O03XFCK03YF8,I07RF8O03YF8I01gFC,I0SF8O03gFI0gGFE,003SF8O03hKF,007SF8O03hKF8,00TF8O03hKFC,01TF8O03hKFE,03TF8O03hLF,07TF8O03hLF8,0UF8O07hLFE,1UF8O07hMF,3UF8O07hMF8,1UF8O07hMFC,0UF8O07hMFE,07TF8O07hMFE,03TF8O07hMFE,01TF8O07hMFC,00TF8O07hMF8,007SF8O07hMF,003SF8O07hLFE,001SF8O07hLFC,I0SF8O07hLF8,I07RF8O07hLF,I03RF8O07hKFE,I01RF8O07hKFC,J0RF8O03hKF8,J07QF8O03gF00gHFE,J03QF8O03YFJ0gGFC,J01QFCO03XF8J01gF8,K0QFCO03WFEL07YF,K07PFCO03WF8L01XFE,K03PFCO03WFN07WFC,K03PFCO03VFEN03WF8,K01PFCO03VFCO0WF,L0PFCO03VF8O07UFE,L07OFCO03VFP03UFC,L03OFEO03UFEP01UF8,L01OFEO03UFEQ0UF,M0OFEO03UFCQ07SFE,M03NFEO03UF8Q07SFC,M03NFEO03UF8Q03SF8,M01NFEO01UF8Q03SF,N0NFEO01UFR01RFE,N07NFO01UFR01RFC,N03NFO01UFS0RF8,N01NFO01TFES0RF,O0NFO01TFES0QFE,O07MFO01TFES0QFC,O03MF8N01TFES0QF8,O01MF8O0TFES0QF,P0MF8O0TFES07OFE,P07LF8O0TFES07OFC,P03LFCO0TFES07OF8,P01LFCO0TFES07OF,Q0LFCO0TFES07NFC,Q07KFCO0TFES07NF8,Q03KFEO07SFES07NF,Q01KFEO07SFES07MFE,R0KFEO07SFES07MFC,R07JFEO07SFES07MF8,R03KFO07TFS07MF,R01KFO03TFS07LFE,S0KFO03TFS07LFC,S07JF8N03TF8R07LF8,S03JF8N03TF8R0MF,S01JF8N01TFCR0LFE,T0JFCN01TFCR0LFC,T07IFCN01TFER0LF8,T03IFCN01TFER0LF,T01IFEO0UFR0KFE,U0IFEO0UF8Q0KFC,U07FFEO0UFCQ0KF8,U03IFO07UFQ0KF,U01IFO07UF8O01JFE,V0IFO07UFEO01JFC,V07FF8N03VF8N01JF8,V03FF8N03WF8M01JF,V01FFCN01XFEL01IFE,W0FFCN01XFEL01IFC,W07FCN01XFEL03IF8,W03FEO0XFCL03IF,W01FEO0XFCL03FFE,X0FFO07WF8L03FF8,X07FO07WF8L07FF,X03F8N03WFM07FE,X01F8N01WFM07FC,Y0F8N01VFEM07F8,Y07CO0VFEM0FF,Y03CO0VFCM0FE,Y01EO07UF8M0FC,g0EO03UFM01F8,g07O01TFEM01F,g038O0TFCM01E,g018O07SF8M03C,gG0CO03SFN038,gG04O01RFEN07,gG02P0RFCN06,gR07QFO0C,gR03PFEO08,gS0PF8,gS03NFE,gT0NF,gT01LFC,gU01JF8,,:::::::::::::::::::::::::::::::::I01FC00CgW07F,001IF83FgV03FFE,007IFE7F8gU0JF8,00KF7F8gT01JFC,01MF8gT03JFC,07MF8gT03JFE,07MF8gT07FFBFE,0NF8gT07FE3FE,1NF8gT07FE3FE,1IFC1IF8gT07FC3FC,3IF007FF8gT07FC1FC,3FFE003FF8007FFCN07EL0F8I07FEJ0FFE0F807FEW03E,3FFC001FF803JF8003FFC1FF8IF07FE001IFC00KF003IFC03IF03IF01FFC1FF8,7FFC001FF807JFE007FFC7FFCIF0IF007JF01KF80KF07IF07IF03FFE3FFE,7FF8001FF80LF007FFCMF1IF80KF81KF81KF87IF07IF03FFE7IF,7FF8001FF81LF007FFCMF3IFC1KFC1KF83KFC7IF07IF07FFEJF,7FF8001FF83LF807FFDMF7IFC3KFE1KF87KFE7IF07IF07FFEJF8,7FFI01FF83FFC7FF807TFE7KFE1KF0MF7IF07IF07MF8,IFJ0FF03FF83FF807TFEIF0IF0JFE0MF7IF07IF03MF8,IFJ07E03FF81FFC03JF1FFEIFC7FEFFC03FF00FFE01IF07FFBBFF03IF01JF8FF8,IFM03FF81FFC003FFE1FF07FF87JF803FF007FC01FFC03FF83FF003FF001IF1FF8,IFM01FF81FFC003FFC1FF07FF87JF801FF807FC03FF801FF81FF003FF001FFE1FF8,IFN0FF01FFC001FFC1FF07FF07FDFF803FF807FC03FF801FFC1FF003FF001FFC0FF,IFN03C03FFC001FF81FE07FF03FBMF807FC03FF801FFC1FF003FF001FFC0FE,IFP01IFC001FF80FC07FE01F3MF807FC03FFI0FFC1FF003FF001FFC07C,IFJ07EI03JFC001FF8J07FEI03MF807FC03FFI0FFC1FF003FF001FFC,IFJ0FF003KFC001FF8J07FEI03MF807FC03FFI0FFC1FF003FF001FF8,IFI01FF80LFC001FF8J07FEI03MF807FC03FFI0FFC1FF003FF001FF8,7FF8001FF81LFC001FF8J07FEI03MF807FC03FFI0FFC1FF003FF001FF8,7FF8001FFC3LFC001FF8J07FEI03MF007FC03FFI0FFC1FF003FF001FF8,7FF8001FFC7LFC001FFK07FEI03FFM07FC03FFI0FFC1FF003FF001FF8,7FFC001FFC7IF1FFC001FFK07FEI03FFM07FC03FFI0FFC1FF003FF001FF8,3FFC001FFCIF01FFC001FFK07FEI03FFI03C007FC03FF801FFC1FF803FF001FF8,3FFE001FFCFFE01FFC001FFK07FEI01FF8007F007FC03FF801FFC1FF807FF001FF8,3IF003FF8FFC01FFC003FFK07FEI01FF800FF00FFC03FF803FF81FF807FF001FF8,1IF80IF8FFE03FFC003FF8J07FEI01FFC01FF80FFE01FFC03FF81FFC0IF001FF8,1NF8IF07FFC033FFBI06FFE6I0IF07FF8CFFEE1FFE07FF81IF3IFB19FFD8,0NF0NFC7JFC01KFI0MF1KF0MF01SFC,07MF0NFE7JFC01KFI07LF1KF0MF01SFE,03LFE07MFE7JFC01KF8007LF1KF07KFE01SFE,01LFC07MFE7JFC01KF8003KFE1KF03KFC00SFE,00LF803MFE7JFC01KFI01KFC1KF01KF800JFCNFE,003JFE001JF1FFE7JFC01KFJ07JF01KF00KFI07IF8NFC,I0JF8I07FFC0FFC3JF800JFEJ01IFE00JFE003IFCI01IF0IF9JFC,I01FFCJ01FF003FY03FEQ07FEK07F8,^FS^XZ");
            RawPrinterHelper.SendStringToPrinter(printerName, strBldr.ToString());



        }


        //public void Print(string printerName)
        //{
        //    if (printerName == null)
        //    {
        //        throw new ArgumentNullException("printerName");
        //    }
        //    StringBuilder sb1 = new StringBuilder();
        //    //^XA=Indicates Starting of Zpl
        //    sb1.AppendLine("^XA");
        //    sb1.AppendLine("^LL350");//^FS
        //    sb1.AppendLine("^PW930");//^FS
        //    sb1.AppendLine("^FO10,10");
        //    sb1.AppendLine("^AQ,80,80");
        //    // sb1.AppendLine("^FDAnja^FS");

        //    //FOa,b
        //    //a=Postion from x-axis
        //    //b=Position from y-axis
        //    //Aa,b,c
        //    //a=Font size Like Q,V,R,0
        //    //b=Font width
        //    //c=Font Height

        //    sb1.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.strFirstName));
        //    sb1.AppendLine("^FO10,68");
        //    sb1.AppendLine("^AQ,80,80");
        //    sb1.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.strLastName));

        //    sb1.AppendLine("^FO10,150");
        //    sb1.AppendLine("^AQ,50,50");
        //    sb1.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.PickUpTime));

        //    sb1.AppendLine("^FO240,150");
        //    sb1.AppendLine("^AQ,50,50");
        //    sb1.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.Cold));
        //    //^PQ2= Indicates number of copies to print
        //    sb1.AppendLine(string.Format(CultureInfo.InvariantCulture, "^PQ{0}", this.strNoOfCopies));
        //    //sb1.AppendLine("^PQ2");
        //    //^XZ=Indicates ending of ZPL page
        //    sb1.AppendLine("^XZ");
        //    //RawPrinterHelper.SendStringToPrinter(printerName, sb1.ToString());
        //    //for (int counter = 0; counter <Convert.ToInt32(this.strNoOfCopies); counter++)
        //    //{
        //    //    RawPrinterHelper.SendStringToPrinter(printerName, sb1.ToString());
        //    //    System.Threading.Thread.Sleep(500);
        //    //}
        //    if (NewMember != "")
        //    {
        //        StringBuilder strb = new StringBuilder();
        //        //strb.AppendLine("^XA");
        //        //strb.AppendLine("^LL350");//^FS
        //        //strb.AppendLine("^PW930");//^FS
        //        //strb.AppendLine("^FO20,30");
        //        //strb.AppendLine("^AQ,80,80");
        //        //strb.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.NewMember));
        //        //strb.AppendLine("^FO20,150");
        //        //strb.AppendLine("^AQ,50,50");
        //        //strb.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.PickUpTime));

        //        //strb.AppendLine("^FO240,150");
        //        //strb.AppendLine("^AQ,50,50");
        //        //strb.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.Cold));
        //        ////^PQ2= Indicates number of copies to print
        //        //strb.AppendLine(string.Format(CultureInfo.InvariantCulture, "^PQ{0}", "1"));
        //        ////sb1.AppendLine("^PQ2");
        //        ////^XZ=Indicates ending of ZPL page
        //        //strb.AppendLine("^XZ");

        //        strb.Append(@"^XA^CFA,45^FO20,20^FB270,15,0,L^FDKRONSOFT^FS^CF0,40^FO30,65^FD");
        //        strb.Append(strFirstName);
        //        strb.Append("^FS");
        //        //strb.Append("^FO80,300^BY4,2.0,75^BQN,2,10^FD093");
        //        //strb.Append(strFirstName);
        //        //strb.Append("^FS");
        //        strb.Append("^FOx,y^A0N,20,20^FB800,10,0,L,0^FD");
        //        strb.Append("^CFA,25^FO120,125^FB270,15,0,L^FD");
        //        strb.Append(strLastName);
        //        strb.Append("^FS");
        //        //strb.Append("^FOx,y^A0N,30,30^FB800,10,0,L,0^FD");
        //        //strb.Append("^CF0,25^FO120,90^FB300,15,0,L^FDPIF: ");
        //        //strb.Append(strNoOfCopies);
        //        //strb.Append("^FS");
        //        //strb.Append("^FS");
        //        strb.Append("^FO20,40^BY4,2.0,65^BQN,2,4^FD   ");
        //        strb.Append(strFirstName);
        //        strb.Append("^FS");
        //        strb.Append("^FO310,2^BY4,2.0,15^BQN,2,4^FD   ");
        //        strb.Append(strFirstName);
        //        strb.Append("^FS^XZ");

        //        RawPrinterHelper.SendStringToPrinter(printerName, strb.ToString());
        //    }
        //}

        public void Print(string printerName)
        {
            if (printerName == null)
            {
                throw new ArgumentNullException("printerName");
            }
            StringBuilder sb1 = new StringBuilder();
            //^XA=Indicates Starting of Zpl
            sb1.AppendLine("^XA");
            sb1.AppendLine("^LL350");//^FS
            sb1.AppendLine("^PW930");//^FS
            sb1.AppendLine("^FO10,10");
            sb1.AppendLine("^AQ,80,80");
            // sb1.AppendLine("^FDAnja^FS");

            //FOa,b
            //a=Postion from x-axis
            //b=Position from y-axis
            //Aa,b,c
            //a=Font size Like Q,V,R,0
            //b=Font width
            //c=Font Height

            sb1.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.invNo));
            sb1.AppendLine("^FO10,68");
            sb1.AppendLine("^AQ,80,80");
            sb1.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.description));

            sb1.AppendLine("^FO10,150");
            sb1.AppendLine("^AQ,50,50");
            sb1.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.purchaseDate));

            sb1.AppendLine("^FO240,150");
            sb1.AppendLine("^AQ,50,50");
            sb1.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.Cold));
            //^PQ2= Indicates number of copies to print
            sb1.AppendLine(string.Format(CultureInfo.InvariantCulture, "^PQ{0}", this.serialNumber));
            //sb1.AppendLine("^PQ2");
            //^XZ=Indicates ending of ZPL page
            sb1.AppendLine("^XZ");
            //RawPrinterHelper.SendStringToPrinter(printerName, sb1.ToString());
            //for (int counter = 0; counter <Convert.ToInt32(this.strNoOfCopies); counter++)
            //{
            //    RawPrinterHelper.SendStringToPrinter(printerName, sb1.ToString());
            //    System.Threading.Thread.Sleep(500);
            //}
            if (NewMember != "")
            {
                StringBuilder strb = new StringBuilder();
                //strb.AppendLine("^XA");
                //strb.AppendLine("^LL350");//^FS
                //strb.AppendLine("^PW930");//^FS
                //strb.AppendLine("^FO20,30");
                //strb.AppendLine("^AQ,80,80");
                //strb.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.NewMember));
                //strb.AppendLine("^FO20,150");
                //strb.AppendLine("^AQ,50,50");
                //strb.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.PickUpTime));

                //strb.AppendLine("^FO240,150");
                //strb.AppendLine("^AQ,50,50");
                //strb.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.Cold));
                ////^PQ2= Indicates number of copies to print
                //strb.AppendLine(string.Format(CultureInfo.InvariantCulture, "^PQ{0}", "1"));
                ////sb1.AppendLine("^PQ2");
                ////^XZ=Indicates ending of ZPL page
                //strb.AppendLine("^XZ");

                //strb.Append(@"^XA^FO10,10^GFA,1300,1300,13,,:::::::::::::::R0FC,Q03FF8,Q07FFC,P01JF,:P03JF802,M04003JF803,M08007JF8018,L018007IFC001C,L07800JF8001E,L0FI0JFI01F,K01FI0IFEI01F8,K03F001IFEI01FC,K07E001IFEI01FE,K0FE001IFEI01FF,J01FE001IFEI01FF8,J03FE001IFEI01FFC,J07FC001IFEI01FFE,J0FFC003JFI03IF,I01FFC003JFI07IF8,I03FFC003JF800JFC,I07FFC003JFE03JFE,001IFC003RF,003IFC003RF8,003IFC003RFC,003IFC003RFE,I0IFC003RFC,I0IFC003RF8,I07FFC003KF0LF,I03FFC003JF803JFE,I01FFC003JFI0JFC,J0FFC003IFEI07IF8,J07FE003IFEI07IF,J03FE003IFEI03FFE,J01FE003IFCI03FFC,K0FE001IFCI03FF8,K07E001IFCI03FF,K03F001IFEI03FC,K01F001IFEI03F8,L0F001IFEI03F,L07800JFI03E,L03800JF8003C,L01800JFE0078,M0C007JF807,M04007JF006,M02003JF004,P01IFE008,Q0IFC,Q07FF,R0FC,,:::::::::001F4O01E,003FCO036,0070CO03,0070C7E39F9C78F8F9E719C,0060CE63IFECC79FDE71FC,00600460CCE7C6318E630E4,006I0E0C0C1FE3386630C,0070C7E0C0C1FC3386630C,0070EC60C0C180318E630C,0038CC71C0C1C6318C678C,003FCFFBE3F0FEF9FC7IF,I0F071K038007038,,:::::::::::^FS^CF0,40^FO130,35^FD");
                //strb.Append(strFirstName);
                //strb.Append("^FS");
                ////strb.Append("^FO80,300^BY4,2.0,75^BQN,2,10^FD093");
                ////strb.Append(strFirstName);
                ////strb.Append("^FS");
                //strb.Append("^FOx,y^A0N,15,15^FB800,10,0,L,0^FD");
                //strb.Append("^CFA,25^FO130,85^FB250,15,0,L^FD");
                //strb.Append(strLastName);
                //strb.Append("^FS");
                ////strb.Append("^FOx,y^A0N,30,30^FB800,10,0,L,0^FD");
                //strb.Append("^CF0,25^FO130,180^FB300,15,0,L^FD PIF: ");
                //strb.Append(strNoOfCopies);
                //strb.Append("^FS");
                ////strb.Append("^FS");
                //strb.Append("^FO20,50^BY4,2.0,65^BQN,2,4^FD  ");
                //strb.Append(strFirstName);
                //strb.Append("^FS^XZ");

                if (description.Length > 140)
                {
                    description = description.Substring(0, 140);
                }


                if (invNo.Length > 0)
                {
                    strb.Append(@"^XA^FO90,10^GFA,1932,1932,28,,::::::::::::::03OFCI01FEI07F8I0FF00E0IFE3IFC7IF,03OFEI07FFC01FFE003FFE0E1IFE7IFC7IF,03OFCI0IFE03IF007IF1E1IFE7IFC7IF8,03OFC001F01C07C0F80F81E1E1EK0F007,03OFC001EJ0F003C1E0061E1EK0E007,03OFC001EJ0E001C1CI01E1EK0E007,03OFCI0F8001E001E3CI01E1EK0E007,03OFCI0FFE01EI0E38J0E1IFI0E007FFC,03OFCI03FFC1CI0E38J0E1IFI0E007FFC,03OFCJ0FFE1CI0E38J0E1IFI0E007FFC,03OFEK03F1EI0E3CJ0E1EK0E007,03OFCL0F1E001E3CI01E1EK0E007,03OFCL070E001C1EI01E1EK0E007,03OFCI0C0070F003C1E0021E1EK0E007,03OFC001F01F07C0F80F81E1E1EK0E007,03OFC001IFE03IF007IF1E1JF00E007IF8,03OFCI07FFC01FFE003FFE1E1JF00E007IF8,03OFCI01FFI03F8I0FF00C0IFE006007IF8,03OFC,03F8L0FC,03F8L0FE,03FBC1E0F1FE,03OFE,03OFEJ0FFC07IF0F00383IF87FFEI0E0038001IFC,03OFEI03FFE07IF8F80383IF87IF001F0038001IFC,03OFEI07IF07IF0F803C3IFC7IF801F0038003IFC,03OFEI0F80707J0FC03C38I07803C03F8038003C,03OFEI0F00207J0FE03C38I07801C03B8038003C,03OFE001EJ07J0EF03C38I07801C079C038003C,03OFE001CJ07J0E783838I07801C071C038001C,03OFE001CJ07FFC0E3C383FFE07803C0F1E038001FFE,03OFE001CJ07FFC0E1E383FFE07IF80E0E038001IF,03OFE001C00387FFC0E1E383FFE07IF81E0F038001FFE,03OFE001C00387J0E0F3838I07FFE01IF038001C,03OFE001E00387J0E07BC38I0783C03IF838003C,03OFE001E00387J0E03FC38I0781E03IF838003C,03OFEI0F00787J0E01FC38I0780F07803C38003C,03OFEI07E1F87IF8E00FC3IFC780F07001C3IF3IFC,03OFEI03IF07IF8E00783IFC78078F001E3IF9IFE,03OFEI01FFE07IF8E00783IFC7803CEI0E3IF9IFE,03OFEJ03F,,::::::::::::^FS^CF0,22^FO10,75^FB380,2,0,C^FD");
                    strb.Append(description);
                    strb.Append("^FS");
                    //strb.Append("^FO80,300^BY4,2.0,75^BQN,2,10^FD093");
                    //strb.Append(strFirstName);
                    //strb.Append("^FS");
                    //strb.Append("^CF0,30^FO100,60^FDSN^FS");

                    //if (strLastName.Length <= 200)
                    //{
                    //    if (!strFirstName.StartsWith("T"))
                    //    {
                    //        strb.Append("^CF0,25^FO0,115^FB400,1,0,C^FD SN: ");
                    //        strb.Append(PickUpTime);
                    //        strb.Append("^FS");
                    //    }

                    //}

                    if (invNo.StartsWith("T"))
                    {
                        strb.Append("^CF0,22^FO10,10^FB400,1,0,L^FD 2020 ^FS");
                    }
                    


                    if (serialNumber.Length > 0)
                    {
                        strb.Append("^CF0,22^FO0,120^FB400,1,0,C^FD SN: ");
                        strb.Append(serialNumber);
                        strb.Append("^FS");
                    }



                    //strb.Append(@"^CFA,20^FO290,20^FD");
                    //strb.Append(strNoOfCopies);
                    //strb.Append("^FS");


                    //strb.Append(@"^FO300,40^BY4,2.0,15^BQN,2,4^FD   ");
                    //strb.Append(strNoOfCopies);
                    //strb.Append("^FS");

                    if (invNo.StartsWith("T"))
                    {
                        strb.Append("^FOx,y^A0N,15,15^FB800,10,0,L,0^FD^CFA,25^FO130,85^FB250,15,0,L^FD^FS^FX Third section with barcode.^CVY^MD15^LH0,0^FO90,140^A0N,18,18^BY2^BCN,25,N,N,Y^FD");
                    }
                    else
                    {
                        strb.Append("^FOx,y^A0N,15,15^FB800,10,0,L,0^FD^CFA,25^FO130,85^FB250,15,0,L^FD^FS^FX Third section with barcode.^CVY^MD15^LH0,0^FO70,140^A0N,18,18^BY2^BCN,25,N,N,Y^FD");
                    }

                    

                  

                    strb.Append(invNo);
                    strb.Append("^FS");
                    //strb.Append("^FS");

                    if (invNo.StartsWith("T"))
                    {
                        strb.Append("^CF0,30^FO150,170^FD400,1,0,C^FD");
                    }
                    else
                    {
                        strb.Append("^CF0,30^FO130,170^FD400,1,0,C^FD");
                    }
                    
                    strb.Append(invNo);
                    strb.Append("^FS^XZ");
                }

                RawPrinterHelper.SendStringToPrinter(printerName, strb.ToString());
            }
        }


    }
}
