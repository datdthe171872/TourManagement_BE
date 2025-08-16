using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Service
{
    public class TourComparisonService : ITourComparisonService
    {
        private readonly MyDBContext _context;

        public TourComparisonService(MyDBContext context)
        {
            _context = context;
        }

        public async Task<TourComparisonResponse> CompareToursAsync(TourComparisonRequest request)
        {
            // Lấy thông tin chi tiết của 2 tour
            var tour1 = await GetTourComparisonInfoAsync(request.Tour1Id);
            var tour2 = await GetTourComparisonInfoAsync(request.Tour2Id);

            if (tour1 == null || tour2 == null)
                throw new Exception("Không tìm thấy một hoặc cả hai tour");

            // So sánh các tiêu chí
            var result = CompareTours(tour1, tour2);

            return new TourComparisonResponse
            {
                Tour1 = tour1,
                Tour2 = tour2,
                Result = result
            };
        }

        private async Task<TourComparisonInfo> GetTourComparisonInfoAsync(int tourId)
        {
            var tour = await _context.Tours
                .Include(t => t.TourOperator)
                .Include(t => t.TourRatings.Where(tr => tr.IsActive))
                .FirstOrDefaultAsync(t => t.TourId == tourId && t.IsActive);

            if (tour == null) return null;

            // Tính điểm đánh giá trung bình
            var averageRating = tour.TourRatings.Any() 
                ? tour.TourRatings.Average(tr => tr.Rating ?? 0) 
                : 0;

            // Tính tỷ lệ đặt chỗ
            var occupancyRate = tour.MaxSlots > 0 
                ? (double)(tour.SlotsBooked ?? 0) / tour.MaxSlots * 100 
                : 0;

            return new TourComparisonInfo
            {
                TourId = tour.TourId,
                Title = tour.Title,
                Description = tour.Description ?? string.Empty,
                DurationInDays = tour.DurationInDays,
                PriceOfAdults = tour.PriceOfAdults,
                PriceOfChildren = tour.PriceOfChildren,
                PriceOfInfants = tour.PriceOfInfants,
                MaxSlots = tour.MaxSlots,
                SlotsBooked = tour.SlotsBooked ?? 0,
                OccupancyRate = Math.Round(occupancyRate, 2),
                AverageRating = Math.Round(averageRating, 1),
                TotalRatings = tour.TourRatings.Count,
                StartPoint = tour.StartPoint ?? string.Empty,
                Transportation = tour.Transportation ?? string.Empty,
                CompanyName = tour.TourOperator?.CompanyName ?? string.Empty
            };
        }

        private ComparisonResult CompareTours(TourComparisonInfo tour1, TourComparisonInfo tour2)
        {
            var comparisons = new List<CriterionComparison>();
            int tour1Wins = 0, tour2Wins = 0, ties = 0;

            // 1. So sánh giá người lớn (giá thấp hơn tốt hơn)
            var priceComparison = CompareCriterion(
                "Giá người lớn", 
                (double)tour1.PriceOfAdults, 
                (double)tour2.PriceOfAdults, 
                false, // Giá thấp hơn tốt hơn
                "VNĐ"
            );
            comparisons.Add(priceComparison);
            if (priceComparison.Winner == "Tour1") tour1Wins++;
            else if (priceComparison.Winner == "Tour2") tour2Wins++;
            else ties++;

            // 2. So sánh thời gian tour (thời gian dài hơn tốt hơn)
            var durationComparison = CompareCriterion(
                "Thời gian tour", 
                ParseDuration(tour1.DurationInDays), 
                ParseDuration(tour2.DurationInDays), 
                true, // Thời gian dài hơn tốt hơn
                "ngày"
            );
            comparisons.Add(durationComparison);
            if (durationComparison.Winner == "Tour1") tour1Wins++;
            else if (durationComparison.Winner == "Tour2") tour2Wins++;
            else ties++;

            // 3. So sánh tỷ lệ đặt chỗ (tỷ lệ cao hơn tốt hơn)
            var occupancyComparison = CompareCriterion(
                "Tỷ lệ đặt chỗ", 
                tour1.OccupancyRate, 
                tour2.OccupancyRate, 
                true, // Tỷ lệ cao hơn tốt hơn
                "%"
            );
            comparisons.Add(occupancyComparison);
            if (occupancyComparison.Winner == "Tour1") tour1Wins++;
            else if (occupancyComparison.Winner == "Tour2") tour2Wins++;
            else ties++;

            // 4. So sánh điểm đánh giá (điểm cao hơn tốt hơn)
            var ratingComparison = CompareCriterion(
                "Điểm đánh giá", 
                tour1.AverageRating, 
                tour2.AverageRating, 
                true, // Điểm cao hơn tốt hơn
                "sao"
            );
            comparisons.Add(ratingComparison);
            if (ratingComparison.Winner == "Tour1") tour1Wins++;
            else if (ratingComparison.Winner == "Tour2") tour2Wins++;
            else ties++;

            // 5. So sánh số lượng đánh giá (nhiều feedback hơn tốt hơn)
            var feedbackComparison = CompareCriterion(
                "Số lượng đánh giá", 
                tour1.TotalRatings, 
                tour2.TotalRatings, 
                true, // Nhiều feedback hơn tốt hơn
                "đánh giá"
            );
            comparisons.Add(feedbackComparison);
            if (feedbackComparison.Winner == "Tour1") tour1Wins++;
            else if (feedbackComparison.Winner == "Tour2") tour2Wins++;
            else ties++;

            // Xác định người thắng cuộc
            string winner;
            if (tour1Wins > tour2Wins)
                winner = $"Tour 1 thắng ({tour1Wins}/{5} tiêu chí)";
            else if (tour2Wins > tour1Wins)
                winner = $"Tour 2 thắng ({tour2Wins}/{5} tiêu chí)";
            else
                winner = $"Hòa ({tour1Wins} - {tour2Wins} - {ties})";

            return new ComparisonResult
            {
                Tour1Wins = tour1Wins,
                Tour2Wins = tour2Wins,
                Ties = ties,
                Winner = winner,
                CriterionComparisons = comparisons
            };
        }

        private CriterionComparison CompareCriterion(string name, double value1, double value2, bool higherIsBetter, string unit)
        {
            string winner;
            string description;

            if (Math.Abs(value1 - value2) < 0.01) // Gần như bằng nhau
            {
                winner = "Tie";
                description = $"Cả hai tour đều có {name} tương đương: {value1} {unit}";
            }
            else if (higherIsBetter)
            {
                if (value1 > value2)
                {
                    winner = "Tour1";
                    description = $"Tour 1 có {name} tốt hơn: {value1} {unit} vs {value2} {unit}";
                }
                else
                {
                    winner = "Tour2";
                    description = $"Tour 2 có {name} tốt hơn: {value2} {unit} vs {value1} {unit}";
                }
            }
            else // lowerIsBetter
            {
                if (value1 < value2)
                {
                    winner = "Tour1";
                    description = $"Tour 1 có {name} tốt hơn: {value1} {unit} vs {value2} {unit}";
                }
                else
                {
                    winner = "Tour2";
                    description = $"Tour 2 có {name} tốt hơn: {value2} {unit} vs {value1} {unit}";
                }
            }

            return new CriterionComparison
            {
                CriterionName = name,
                Winner = winner,
                Description = description,
                Tour1Value = value1,
                Tour2Value = value2
            };
        }

        private double ParseDuration(string duration)
        {
            // Xử lý chuỗi thời gian như "3 ngày", "5 ngày", etc.
            if (string.IsNullOrEmpty(duration)) return 0;
            
            var parts = duration.Split(' ');
            if (parts.Length > 0 && double.TryParse(parts[0], out double result))
                return result;
            
            return 0;
        }
    }
}
