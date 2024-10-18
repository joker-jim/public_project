#include "opencv2/opencv.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include <iostream>

using namespace cv;
using namespace std;

void cvHaarWavelet(Mat &src, Mat &dst, int NIter)
{
    float c;
    Mat tmp = src.clone();
    int width = src.cols;
    int height = src.rows;
    for (int k = 0; k < NIter; k++)
    {
        width /= 2;
        height /= 2;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                c = (tmp.at<float>(2 * y, 2 * x) + tmp.at<float>(2 * y, 2 * x + 1) + tmp.at<float>(2 * y + 1, 2 * x) + tmp.at<float>(2 * y + 1, 2 * x + 1)) / 4;
                dst.at<float>(y, x) = c;
                c = (tmp.at<float>(2 * y, 2 * x) + tmp.at<float>(2 * y, 2 * x + 1) - tmp.at<float>(2 * y + 1, 2 * x) - tmp.at<float>(2 * y + 1, 2 * x + 1)) / 4;
                dst.at<float>(y + height, x) = c;
                c = (tmp.at<float>(2 * y, 2 * x) - tmp.at<float>(2 * y, 2 * x + 1) + tmp.at<float>(2 * y + 1, 2 * x) - tmp.at<float>(2 * y + 1, 2 * x + 1)) / 4;
                dst.at<float>(y, x + width) = c;
                c = (tmp.at<float>(2 * y, 2 * x) - tmp.at<float>(2 * y, 2 * x + 1) - tmp.at<float>(2 * y + 1, 2 * x) + tmp.at<float>(2 * y + 1, 2 * x + 1)) / 4;
                dst.at<float>(y + height, x + width) = c;
            }
        }
        tmp = dst.clone();
        Mat waveletImg;
        normalize(dst, waveletImg, 0, 255, NORM_MINMAX, CV_8UC1);
        imshow("Wavelet Iteration " + to_string(k + 1), waveletImg);
        waitKey(0);
    }
}

void cvInvHaarWavelet(Mat& src, Mat& dst, int NIter) 
{
    Mat tmp = src.clone();
    int width = src.cols;
    int height = src.rows;
    int h, w;

    for (int l = 0; l < NIter; l++)
    {
        h = height / pow(2, NIter - l);
        w = width / pow(2, NIter - l);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                dst.at<float>(2 * y, 2 * x) = (tmp.at<float>(y, x) + tmp.at<float>(y + h, x) + tmp.at<float>(y, x + w) + tmp.at<float>(y + h, x + w)) / 2;
                dst.at<float>(2 * y, 2 * x + 1) = (tmp.at<float>(y, x) - tmp.at<float>(y + h, x) + tmp.at<float>(y, x + w) - tmp.at<float>(y + h, x + w)) / 2;
                dst.at<float>(2 * y + 1, 2 * x) = (tmp.at<float>(y, x) + tmp.at<float>(y + h, x) - tmp.at<float>(y, x + w) - tmp.at<float>(y + h, x + w)) / 2;
                dst.at<float>(2 * y + 1, 2 * x + 1) = (tmp.at<float>(y, x) - tmp.at<float>(y + h, x) - tmp.at<float>(y, x + w) + tmp.at<float>(y + h, x + w)) / 2;
            }
        }

        tmp = dst.clone();
    }
}



int main()
{
    
    Mat img = imread("gray.jpg", IMREAD_GRAYSCALE);
    Mat originalImg = img.clone();

    if (img.empty()) {
        cout << "Cannot load image!" << endl;
        return -1;
    }

    img.convertTo(img, CV_32FC1);
    img /= 255;

    Mat result = Mat::zeros(img.rows, img.cols, CV_32FC1);
    Mat invResult = Mat::zeros(img.rows, img.cols, CV_32FC1);

    
    imshow("Original Image", originalImg);
    waitKey(0);

    Mat floatImg;
    img.convertTo(floatImg, CV_8UC1, 255);
    imshow("Float Image", floatImg);
    waitKey(0);

    cvHaarWavelet(img, result, 3);
    cvInvHaarWavelet(result, invResult, 3);
    
    normalize(invResult, invResult, 0, 255, NORM_MINMAX, CV_8UC1);
    imshow("Inverse Wavelet Image", invResult);
    waitKey(0);

    imwrite("Wavelet Principle.jpg", invResult);

    return 0;
}
