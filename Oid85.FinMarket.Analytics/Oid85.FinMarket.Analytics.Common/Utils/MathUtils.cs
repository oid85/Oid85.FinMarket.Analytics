namespace Oid85.FinMarket.Analytics.Common.Utils;

public static class MathUtils
{
	/// <summary>
	/// Корреляция по формуле Пирсона
	/// </summary>
	public static double Correlation(this List<double> valuesX, List<double> valuesY)
	{
		if (valuesX.Count != valuesY.Count)
			return 0.0;

		double averageX = valuesX.Average();
		double averageY = valuesY.Average();

		double sum = 0.0;
		double sumX = 0.0;
		double sumY = 0.0;

		for (int i = 0; i < valuesX.Count; i++)
		{
			sum += (valuesX[i] - averageX) * (valuesY[i] - averageY);
			sumX += (valuesX[i] - averageX) * (valuesX[i] - averageX);
			sumY += (valuesY[i] - averageY) * (valuesY[i] - averageY);
		}

		return sum / Math.Sqrt(sumX * sumY);
	}  	
}