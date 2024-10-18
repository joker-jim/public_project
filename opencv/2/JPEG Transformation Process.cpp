#include <opencv2/opencv.hpp>
#include <opencv2/highgui/highgui.hpp>

using namespace cv;

int main(int argc, char** argv)
{

    Mat img = imread("gray.jpg", IMREAD_GRAYSCALE);

    if(img.empty())
    {
        std::cout << "Could not open or find the image" << std::endl;
        return -1;
    }

    Mat imgFloat;
    img.convertTo(imgFloat, CV_32F);  

    Mat dct_img, idct_img;

    dct(imgFloat, dct_img);

    Mat dct_img_shifted = dct_img.clone();
    dct_img_shifted += Scalar::all(128);
    log(dct_img_shifted, dct_img_shifted); 
    normalize(dct_img_shifted, dct_img_shifted, 0, 255, NORM_MINMAX, CV_8U);

    idct(dct_img, idct_img);
    idct_img.convertTo(idct_img, CV_8U);

    imwrite("JPEG Transformation.jpg", idct_img);

    namedWindow("Original Image", WINDOW_NORMAL);
    imshow("Original Image", img);

    namedWindow("DCT Transform", WINDOW_NORMAL);
    imshow("DCT Transform", dct_img_shifted);

    namedWindow("Reconstructed Image", WINDOW_NORMAL);
    imshow("Reconstructed Image", idct_img);

    waitKey(0);

    return 0;
}
