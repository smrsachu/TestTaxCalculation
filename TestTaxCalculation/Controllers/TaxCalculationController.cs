using Microsoft.AspNetCore.Mvc;
using System.Xml;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestTaxCalculation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxCalculationController : ControllerBase
    {
        // POST api/<TaxCalculationController>
        [HttpPost]
        public IActionResult CalculateSalesTax(string emailContent,float taxRate)
        { 
            try 
            { 
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(emailContent); 
                string costCentreName = "";
                XmlNode costCentre= xmlDoc.SelectSingleNode("//cost_centre");
                if (costCentre != null)
                {
                    if(string.IsNullOrEmpty( costCentre.InnerText))
                        costCentreName = "UNKNOWN";
                    else
                    costCentreName = costCentre.InnerText;
                }
                else
                    costCentreName = "UNKNOWN";
                XmlNode totalNode = xmlDoc.SelectSingleNode("//total");
                if (totalNode != null) 
                { 
                    string totalValue = totalNode.InnerText.Replace(",",""); 
                    float tRate = taxRate / 100;
                    int totalAmountWithTax = Convert.ToInt32(totalValue);

                    //float salesTaxAmount = totalAmount * tRate;
                    float amountWithoutTax = (totalAmountWithTax / (1 + tRate));
                    float salesTaxAmount = totalAmountWithTax - amountWithoutTax;

                    var result = new {
                        Message = "Sales Tax Calculation Done",
                        TaxincludedAmount = totalAmountWithTax,
                        SalesTax = salesTaxAmount, 
                        TaxExcludedAmount = amountWithoutTax ,
                        CostCentreName = costCentreName
                    }; 
                    return Ok(result);
                } 
                else 
                { 
                    var result = new { 
                        Message = "Failed - Total tag not found in email content" 
                    }; 
                    return Ok(result); 
                } 
            } 
            catch (XmlException ex)
            { 
                var result = new { 
                    Message = "Failed - XML is not well - formed " + ex.Message }
                ; 
                return Ok(result);
            }
        } 
    } 
}
