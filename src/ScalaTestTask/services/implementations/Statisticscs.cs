using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Models.Regression.Linear;
using System;
namespace ScalaTestTask.services.implementations
{
    public class AccordStatistics 
    {
        public double Mean(double[] observations, double[] weights)
        {
            var normal = FitNormalDistribution(observations, weights);
            return normal.Mean;
        }
        public double Variance(double[] observations, double[] weights)
        {
            var normal = FitNormalDistribution(observations, weights);
            return normal.Variance;
        }
        public double StandardDeviation(double[] observations, double[] weights)
        {
            var normal = FitNormalDistribution(observations, weights);
            return normal.StandardDeviation;
        }
        public object LinearRegression(double[] observations, double[] weights)
        {
            // https://github.com/accord-net/framework/wiki/Linear-Support-Vector-Machines
            var x = Enumerable.Range(1, observations.Length).Select(v => (double)v).ToArray();

            // Use Ordinary Least Squares to learn the regression
            OrdinaryLeastSquares ols = new OrdinaryLeastSquares();

            // Use OLS to learn the simple linear regression
            SimpleLinearRegression regression = ols.Learn(x, observations,weights);

            var obj = new { expression = regression.ToString(), k = regression.Slope, b = regression.Intercept };
            return obj;
        }

        private NormalDistribution FitNormalDistribution(double[] observations, double[] weights)
        {
            // Now, in order to fit a distribution to those values, 
            // all we have to do is to create a base distribution:
            var normal = new NormalDistribution();
            // the variance may be close to zero
            var options = new NormalOptions() { Regularization = double.Epsilon };
            // and then pass those values to its .Fit method:
            normal.Fit(observations, weights, options);
            return normal;
        }
    }
}
