#include <opencv2/opencv.hpp>
#include <opencv2/highgui/highgui.hpp>

using namespace cv;

int main(int argc, char** argv)
{
    Mat img = imread("gray.jpg", IMREAD_GRAYSCALE);
    int width = img.cols - img.cols % 8;
    int height = img.rows - img.rows % 8;

    Rect roi(0, 0, width, height);
    img = img(roi);

    if (img.empty())
    {
        std::cout << "Could not open or find the image" << std::endl;
        return -1;
    }

    Mat imgFloat;
    img.convertTo(imgFloat, CV_32F);

    Mat dct_img;
    dct(imgFloat, dct_img);

    Mat quantization_matrix = (Mat_<float>(8, 8) <<
        16,  11,  10,  16,  24,  40,  51,  61,
        12,  12,  14,  19,  26,  58,  60,  55,
        14,  13,  16,  24,  40,  57,  69,  56,
        14,  17,  22,  29,  51,  87,  80,  62,
        18,  22,   37,  56,  68, 109, 103,  77,
        24,  35,   55,  64,  81, 104, 113,  92,
        49,  64,   78,  87, 103, 121, 120, 101,
        72,  92,   95,  98, 112, 100, 103,  99);

    for (int i = 0; i < dct_img.rows; i += 8)
    {
        for (int j = 0; j < dct_img.cols; j += 8)
        {
            Mat block = dct_img(Rect(j, i, 8, 8));
            divide(block, quantization_matrix, block);

            for (int x = 0; x < block.rows; ++x)
            {
                for (int y = 0; y < block.cols; ++y)
                {
                    block.at<float>(x, y) = std::round(block.at<float>(x, y));
                }
            }

            multiply(block, quantization_matrix, block);
        }
    }

    Mat idct_img;
    idct(dct_img, idct_img);
    idct_img.convertTo(idct_img, CV_8U);

    imwrite("JPEG Principle.jpg", idct_img);

    namedWindow("Original Image", WINDOW_NORMAL);
    imshow("Original Image", img);

    namedWindow("DCT Image", WINDOW_NORMAL);
    imshow("DCT Image", dct_img);

    namedWindow("Quantized Image", WINDOW_NORMAL);
    imshow("Quantized Image", idct_img);

    waitKey(0);

    return 0;
}
