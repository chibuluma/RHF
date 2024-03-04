using System;

public class ContributionReceiptGenerator
{
    public static string GenerateReceiptEmail(string contributorName, double contributionAmount, DateTime? paymentDate)
    {

        string body = $@"<html>
                           <body>
                               <p>Dear {contributorName},</p>
                               <p>We want to express our sincere gratitude for your generous contribution of K{contributionAmount:F2}.</p>
                               <p>Your support is invaluable and helps us in our mission to plow back into our community.</p>
                               <p><strong>Transaction Details:</strong></p>
                               <ul>
                                   <li>Amount: K{contributionAmount:F2}</li>
                                   <li>Date: {paymentDate:yyyy-MM-dd}</li>
                               </ul>
                               <p>Thank you once again for your commitment and generosity. We truly appreciate your support.</p>
                               <p>Best regards,<br/>Royal Hands Foundation</p>
                           </body>
                       </html>";

        return body;
    }
}
