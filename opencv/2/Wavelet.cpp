#include <iostream>
#include <opencv2/opencv.hpp>

using namespace std;
using namespace cv;


void waveletCompression(Mat& image, int level)
{
    Mat temp = image.clone();
    int width = image.cols;
    int height = image.rows;

    cvtColor(temp, temp, COLOR_BGR2GRAY);

    temp.convertTo(temp, CV_32F, 1.0 / 255.0);

    for (int i = 0; i < level; i++)
    {
        pyrDown(temp, temp);
        pyrUp(temp, temp);
    }
    resize(temp, temp, Size(width, height));
    temp.convertTo(temp, CV_8U, 255.0);
    cvtColor(temp, image, COLOR_GRAY2BGR);
}

int main()
{
    
    Mat image = imread("gray.jpg");

    
    if (image.empty())
    {
        cout << "Cannot load image!" << endl;
        return -1;
    }

    namedWindow("Original Image", WINDOW_NORMAL);
    imshow("Original Image", image);

   
    int level = 3;
    waveletCompression(image, level);

    namedWindow("Compressed Image", WINDOW_NORMAL);
    imshow("Compressed Image", image);

    imwrite("wavelet.jpg", image);


    waitKey(0);

    return 0;
}
