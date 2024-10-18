#include <opencv2/opencv.hpp>

int main() {
 
    cv::Mat originalImage = cv::imread("gray.jpg", cv::IMREAD_COLOR);
    if (originalImage.empty()) {
        printf("Cannot load image!");
        return -1;
    }

    cv::imshow("Original Image", originalImage);
    cv::waitKey(0);

    std::vector<int> compression_params;
    compression_params.push_back(cv::IMWRITE_JPEG_QUALITY);
    compression_params.push_back(95);
    if (!cv::imwrite("JPEG.jpg", originalImage, compression_params)) {
        printf("Cannot save image!");
        return -1;
    }

    cv::Mat processedImage = cv::imread("JPEG.jpg", cv::IMREAD_COLOR);
    if (processedImage.empty()) {
        printf("Cannot load processed image!");
        return -1;
    }

    cv::imshow("Processed Image", processedImage);
    cv::waitKey(0);

    return 0;
}