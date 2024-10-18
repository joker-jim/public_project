#include <opencv2/opencv.hpp>

int main()
{
    cv::Mat image = cv::imread("lena.jpg", cv::IMREAD_GRAYSCALE);

    if (image.empty())
    {
        std::cout << "Cannot load image!" << std::endl;
        return 1;
    }

    cv::imwrite("gray.jpg", image);

    std::cout << "image save!" << std::endl;

    return 0;
}